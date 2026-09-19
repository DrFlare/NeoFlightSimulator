using AI;
using UnityEngine;

namespace PlaneInput
{
    public class AIPlaneInput : IPlaneInput
    {
        private NeuralNet _net;
        private float[] _outputs = { 0f, 0f, 0f, 0f };

        public AIPlaneInput()
        {
            _net = new NeuralNet();
        }

        public AIPlaneInput(string weightsPath)
        {
            _net = new NeuralNet(weightsPath);
        }

        public void Tick( Vector3 ringPos)
        {
            float[] packed =
            {
                ringPos.x, ringPos.y, ringPos.z
            };

            _outputs = _net.forward(packed);
        }

        public float GetHorizontal()
        {
            return _outputs[2];
        }

        public float GetVertical()
        {
            return _outputs[0];
        }

        public float GetRudder()
        {
            return _outputs[1];
        }

        public float GetThrust()
        {
            if (_outputs.Length < 4)
            {
                return 0;
            }

            return _outputs[3];
        }

        public NeuralNet Net => _net;
    }
}