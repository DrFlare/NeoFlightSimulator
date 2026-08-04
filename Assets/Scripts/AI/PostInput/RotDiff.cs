using System;
using UnityEngine;

namespace FlightSimulator.AI
{
    public class RotDiff : Layer
    {
        private float diff;
        private float QA;       // old
        private Quaternion RR;  // old
        
        private Quaternion NR;  //old + new
        private Vector3 NF;
        private Vector3 PRV;

        public float[] forward(float[] input)
        {
            // ne koristiti
            throw new System.NotImplementedException();
        }

        public float calculateRotDiff(Vector3 planePos, Vector3 ringPos, UpdateRotation nextPlaneRot)
        {
            // new Forward
            NF = nextPlaneRot.NewRotation * Vector3.forward;
            // Plane - Ring Vector
            PRV = ringPos - planePos;
            // new Rotation
            NR = nextPlaneRot.NewRotation;

            diff = Vector3.Angle(NF, PRV) / 360f;
            return diff;
        }
        
        public float calculateRotDiffOld(Quaternion ringRot, UpdateRotation nextPlaneRot, Quaternion planeRot)
        {
            // ne koristiti
            RR = ringRot;
            NR = nextPlaneRot.NewRotation;
            QA = Quaternion.Angle(ringRot, nextPlaneRot.NewRotation);
            
            diff = (float)(Math.Pow(QA, 2) 
                           - Math.Pow(Quaternion.Angle(ringRot, planeRot), 2));
            return diff;
        }

        public float[] backward(float[] grads)
        {
            var num = (float)Math.Sqrt((double)NF.sqrMagnitude * (double)PRV.sqrMagnitude);
            
            // d L / d Fake ReLU
            var grad = num < 1.0000000036274937E-15 ? 0f : grads[0] / 360f;

            // d L / d ACOS
            grad = grad * 57.29578f;

            var dot = Vector3.Dot(NF, PRV) / num;
            
            var cl = Mathf.Clamp(dot , -1f, 1f);
            
            // d L / d Clamp
            grad = -grad / (float)Math.Sqrt(1 - Math.Pow(cl, 2));
            
            // Clamp(x, -1, 1) = max(-1, min(1, x))
            // d L / d Dot
            grad = dot is >= -1 and <= 1 ? grad : 0;

            var dLdNF = grad * PRV;
            
            var dNFdNR = new[]
            {
                new[] { 2 * NR.y, 2 * NR.z, 2 * NR.w, 2 * NR.x },
                new[] { -2 * NR.x, -2 * NR.w, 2 * NR.z, 2 * NR.y },
                new[] { 0, -4 * NR.x, -4 * NR.y, 0 }
            }; // 3x4
            
            
            var dLdNR = new[] { 0f, 0f, 0f, 0f };

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    dLdNR[j] += dLdNF[i] * dNFdNR[i][j];
                }
            }

            return dLdNR; // 1x4
        }

        public float[] backwardOld(float[] grads)
        {
            if (QA == 0)
            {
                return new [] {0f, 0f, 0f, 0f}; // samo bolje
            }
            
            var dLdQA = grads[0] * 2 * QA;
            var dLdAC = dLdQA * 2.0 * 57.295780181884766;

            var dot = Quaternion.Dot(RR, NR);
            var num = Math.Min(Math.Abs(dot), 1f);

            var dLdN = - dLdAC / Math.Sqrt(1 - Math.Pow(num, 2));
            var dLdM = dLdN * (Math.Abs(dot) <= 1 ? 0 : 1);

            var dLdQD = dLdM * dot / Math.Abs(dot);
            var dLdNR = new float[4];

            for (int i = 0; i < 4; i++)
            {
                dLdNR[i] = (float) dLdQD * RR[(i + 3) % 4]; // popravljanje Q[] :(
            }

            return dLdNR;
        }

        public float Diff => diff;
    }
}