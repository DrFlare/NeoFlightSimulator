using UnityEngine;

namespace FlightSimulator.AI
{
    public class UpdatePosition : Layer
    {
        private float velocity;
        private Quaternion rotation;
        private Vector3 newPosition;
        private Vector3 newForward;
        private float thrust;

        public UpdatePosition(float velocity)
        {
            this.velocity = velocity;
        }

        public Vector3 calculateNextPos(Vector3 initial, Quaternion newRotation, float thrust)
        {
            this.thrust = thrust;
            rotation = newRotation;

            newForward = (newRotation * Vector3.forward).normalized;
            newPosition = initial + newForward * (velocity * (1 + (thrust + 0.5f)));
            return newPosition;
        }

        public float[] forward(float[] input)
        {
            // ne koristiti
            throw new System.NotImplementedException();
        }

        public float[] backward(float[] grads)
        {
            var dLdNF = new float[3];

            for (int i = 0; i < 3; i++)
            {
                dLdNF[i] = grads[i] * velocity * (1 + (thrust + 0.5f));
            }

            var dNFdNR = new[]
            {
                new[] { 2 * rotation.y, 2 * rotation.z, 2 * rotation.w, 2 * rotation.x },
                new[] { -2 * rotation.x, -2 * rotation.w, 2 * rotation.z, 2 * rotation.y },
                new[] { 0, -4 * rotation.x, -4 * rotation.y, 0 }
            }; // 3x4

            var dLdNR = new[] { 0f, 0f, 0f, 0f };

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    dLdNR[j] += dLdNF[i] * dNFdNR[i][j];
                }
            }

            return dLdNR;
        }

        public float backwardThrust(float[] grads)
        {
            return Vector3.Dot(new Vector3(grads[0], grads[1], grads[2]), newForward * velocity);
        }

        public Vector3 NewPosition => newPosition;

        public float Velocity => velocity;
    }
}