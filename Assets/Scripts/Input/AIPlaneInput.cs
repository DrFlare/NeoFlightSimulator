using FlightSimulator.AI;
using UnityEngine;

namespace FlightSimulator
{
    public class AIPlaneInput : PlaneInput
    {
        private NeuralNet net;
        private float[] outputs = { 0f, 0f, 0f, 0f };

        public AIPlaneInput()
        {
            net = new NeuralNet();
        }

        public AIPlaneInput(string weightsPath)
        {
            net = new NeuralNet(weightsPath);
        }

        public void tick( Vector3 ringPos)
        {
            float[] packed =
            {
                ringPos.x, ringPos.y, ringPos.z
            };

            outputs = net.forward(packed);
        }

        public float getHorizontal()
        {
            return outputs[2];
            // return 0;
        }

        public float getVertical()
        {
            return outputs[0];
        }

        public float getRudder()
        {
            return outputs[1];
        }

        public float getThrust()
        {
            if (outputs.Length < 4)
            {
                return 0;
            }

            return outputs[3];
        }

        public NeuralNet Net => net;
    }
}