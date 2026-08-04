using System;

namespace FlightSimulator.AI
{
    public class Softmax : Layer
    {
        private float[] pred;
        private float[] labels;
        
        public float[] forward(float[] input)
        {
            var sumExp = 0.0;
            foreach (var elem in input)
            {
                sumExp += Math.Exp(elem);
            }

            var copy = new float[input.Length];

            for (int i = 0; i < copy.Length; i++)
            {
                copy[i] = (float)(Math.Exp(input[i]) / sumExp);
            }

            pred = copy;
            return copy;
        }

        public float[] backward(float[] grads)
        {
            var copy = new float[pred.Length];

            for (int i = 0; i < pred.Length; i++)
            {
                copy[i] = labels[i] - pred[i];
            }

            return copy;
        }

        public float[] Labels
        {
            get => labels;
            set => labels = value;
        }
    }
}