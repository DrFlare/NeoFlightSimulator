using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace FlightSimulator.AI
{
    public class DistDiff : Layer
    {
        private Vector3 diff;
        private Vector3 planePos;
        private Vector3 nextRingPos;
        private Vector3 ringPos; 
        
        public float[] forward(float[] input)
        {
            // ne koristiti
            throw new System.NotImplementedException();
        }

        public float calculateDistDiffBad(UpdatePosition nextPlanePos, Vector3 ringPos, Vector3 planePos)
        {
            // ne koristiti, ovo je samo dist, ne distdiff
            this.ringPos = ringPos;
            diff = nextPlanePos.NewPosition - ringPos;
            return diff.sqrMagnitude;
        }

        public float calculateDistDiff(Vector3 planePos, UpdatePosition nextPlanePos, Vector3 ringPos)
        {
            this.ringPos = ringPos;
            diff = nextPlanePos.NewPosition - ringPos;

            if (planePos.Equals(ringPos))
            {
                return 0;
            }
            
            return diff.magnitude - (planePos - ringPos).magnitude + (nextPlanePos.Velocity * 2.5f);
        }

        public float calculateDistDiffNew(Vector3 ringPos, Vector3 nextRingPos)
        {
            // ne koristiti
            this.ringPos = ringPos;
            return nextRingPos.magnitude - ringPos.magnitude;
        }

        public float[] backward(float[] grads)
        {
            if (planePos.Equals(ringPos))
            {
                return new[]{0f, 0f, 0f};
            }

            
            var temp = (grads[0] / diff.magnitude) * diff;
            
            return new[]{temp.x, temp.y, temp.z};
        }

        public float[] backwardNew(float[] grads)
        {
            // ne koristiti
            var temp = grads[0] / nextRingPos.magnitude * nextRingPos;
            return new[]{temp.x, temp.y, temp.z};
        }

        public Vector3 Diff => diff;
    }
}