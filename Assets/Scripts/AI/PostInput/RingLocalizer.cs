using System;
using UnityEngine;

namespace FlightSimulator.AI
{
    public class RingLocalizer : Layer
    {
        private Vector3 planePos;
        private UpdatePosition nextPlanePos;
        private UpdateRotation nextPlaneRot;
        private Quaternion planeRot;
        private Vector3 ringPos;
        
        public Vector3 getLocalRingPos(Ring ring, Vector3 planePos, Quaternion planeRot)
        {
            var localPos = ring == null ? 
                Vector3.zero : 
                Quaternion.Inverse(planeRot) * (ring.Pose.position - planePos);
         
            return localPos;
        }

        public float[] forward(float[] input)
        {
            // ne koristiti
            throw new System.NotImplementedException();
        }

        public float[] backward(float[] grads)
        {
            // IPAK NE KORISTI SE JER NIJE DIO LOSS FUNC
            
            // QI (X) (RP - NP)
            var denominator = Math.Pow(
                Math.Sqrt(nextPlaneRot.NewRotation.x * nextPlaneRot.NewRotation.x +
                          nextPlaneRot.NewRotation.y * nextPlaneRot.NewRotation.y +
                          nextPlaneRot.NewRotation.z * nextPlaneRot.NewRotation.z +
                          nextPlaneRot.NewRotation.w * nextPlaneRot.NewRotation.w), 3);
            
            // d Quaternion.Inverse / d New Rotation
            var dQIdNR = new[]
            {
                nextPlaneRot.NewRotation.w * nextPlaneRot.NewRotation.w / denominator,
                nextPlaneRot.NewRotation.x * nextPlaneRot.NewRotation.x / denominator,
                nextPlaneRot.NewRotation.y * nextPlaneRot.NewRotation.y / denominator,
                nextPlaneRot.NewRotation.z * nextPlaneRot.NewRotation.z / denominator
            };
            return null;
        }
    }
}