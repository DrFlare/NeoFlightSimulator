using System;
using System.Collections.Generic;
using System.Numerics;

namespace FlightSimulator.AI
{
    public class ReLU : Layer
    {
        private float[] input;
        
        public float[] forward(float[] input)
        {
            this.input = input;
            float[] copy = new float[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                copy[i] = Math.Max(input[i], 0);
            }

            return copy;
        }

        public float[] backward(float[] grads)
        {
            float[] copy = new float[grads.Length];
            for (int i = 0; i < copy.Length; i++)
            {
                copy[i] = input[i] > 0 ? grads[i] : 0;
            }

            return copy;
        }
    }
}