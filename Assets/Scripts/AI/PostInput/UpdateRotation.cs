using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Matrix4x4 = System.Numerics.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace FlightSimulator.AI
{
    public class UpdateRotation : Layer
    {
        private float yawSpeed, pitchSpeed, rollSpeed;

        private Quaternion planeRot;
        private Quaternion newRotation;
        private float x, y, z;

        public UpdateRotation(float yawSpeed, float pitchSpeed, float rollSpeed)
        {
            this.yawSpeed = yawSpeed;
            this.pitchSpeed = pitchSpeed;
            this.rollSpeed = rollSpeed;
        }

        public Quaternion calculateNextRot(Quaternion initial, float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            planeRot = initial;
            newRotation = initial * Quaternion.Euler(x * pitchSpeed, y * yawSpeed, -z * rollSpeed);
            return newRotation;
        }

        public float[] forward(float[] input)
        {
            throw new System.NotImplementedException();
        }

        public float[] backward(float[] grads)
        {
            
            var dNRdQE = new float[4][];
            
            // smjer: prvi indeks indeksira NewRotation, drugi indeksira Q.E
            // dNR.W/dQ.E.WXYZ
            dNRdQE[0] = new []{planeRot.w, -planeRot.x, -planeRot.y, -planeRot.z};
            // dNR.X/dQ.E.WXYZ
            dNRdQE[1] = new []{planeRot.x, planeRot.w, -planeRot.z, planeRot.y};
            // dNR.Y/dQ.E.WXYZ
            dNRdQE[2] = new []{planeRot.y, planeRot.z, planeRot.w, -planeRot.x};
            // dNR.Z/dQ.E.WXYZ
            dNRdQE[3] = new []{planeRot.z, -planeRot.y, planeRot.x, planeRot.w};
            
            var dLdQE = new []{0f, 0f, 0f, 0f}; // 1x4

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    dLdQE[i] += grads[i] * dNRdQE[i][j];
                }
            }

            // d Quaternion.Euler(x, y, z) / d (x, y, z)  -  I = (x, y, z) ################
            var xInput = x * pitchSpeed * Mathf.Deg2Rad;
            var yInput = y * yawSpeed * Mathf.Deg2Rad;
            var zInput = - z * rollSpeed * Mathf.Deg2Rad;

            var sines = new[] { 
                new[]
                {
                    Math.Sin(xInput * 0.5), 
                    Math.Sin(yInput * 0.5), 
                    Math.Sin(zInput * 0.5)
                }, 
                new[]
                {
                    Math.Cos(xInput * 0.5) * 0.5, 
                    Math.Cos(yInput * 0.5) * 0.5, 
                    Math.Cos(zInput * 0.5) * 0.5
                } 
            };
            var cosines = new[]
            {
                new[]
                {
                    Math.Cos(xInput * 0.5), 
                    Math.Cos(yInput * 0.5), 
                    Math.Cos(zInput * 0.5)
                }, 
                new[]
                {
                    -Math.Sin(xInput * 0.5) * 0.5, 
                    -Math.Sin(yInput * 0.5) * 0.5, 
                    -Math.Sin(zInput * 0.5) * 0.5
                }
            };
            
            Func<int, int, int> d = (a, b) => a == b ? 1 : 0;
            
            var dQW = new float[3];
            
            for (var i = 0; i < 3; i++)
            {
                double a = 1f, b = 1f;
            
                for (var j = 0; j < 3; j++)
                {
                    a *= cosines[d(i, j)][j];
                    b *= sines[d(i, j)][j];
                }
            
                dQW[i] = (float)(a + b);
            }
            
            var dQX = new float[3];
            
            for (int i = 0; i < 3; i++)
            {
                var a = sines[d(i, 0)][0] * cosines[d(i, 1)][1] * cosines[d(i, 2)][2];
                var b = cosines[d(i, 0)][0] * sines[d(i, 1)][1] * sines[d(i, 2)][2];
            
                dQX[i] = (float)(a + b);
            }
            
            var dQY = new float[3];
            
            for (int i = 0; i < 3; i++)
            {
                var a = cosines[d(i, 0)][0] * sines[d(i, 1)][1] * cosines[d(i, 2)][2];
                var b = sines[d(i, 0)][0] * cosines[d(i, 1)][1] * sines[d(i, 2)][2];
            
                dQY[i] = (float)(a - b);
            }
            
            var dQZ = new float[3];
            
            for (int i = 0; i < 3; i++)
            {
                var a = cosines[d(i, 0)][0] * cosines[d(i, 1)][1] * sines[d(i, 2)][2];
                var b = sines[d(i, 0)][0] * sines[d(i, 1)][1] * cosines[d(i, 2)][2];
            
                dQZ[i] = (float)(a - b);
            }

            // d Quaternion.Euler(X,Y,Z) / d (X,Y,Z) GOTOV ################################

            var dQEdI = new []{
                dQW, dQX, dQY, dQZ
            };
            
            // d L / d Input
            var dLdI = new [] {0f, 0f, 0f}; // 1x3 mat

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    dLdI[i] += dLdQE[j] * (float)dQEdI[j][i];
                }
            }

            var dIdXYZ = new[] {pitchSpeed * Mathf.Deg2Rad, yawSpeed * Mathf.Deg2Rad, - rollSpeed * Mathf.Deg2Rad};

            var dLdXYZ = new float[3];

            for (var i = 0; i < 3; i++)
            {
                dLdXYZ[i] = dLdI[i] * dIdXYZ[i];
            }

            return dLdXYZ;
        }

        public Quaternion NewRotation => newRotation;
    }
}