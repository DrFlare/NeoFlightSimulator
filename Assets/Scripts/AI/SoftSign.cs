using System;
using UnityEngine;

namespace FlightSimulator.AI
{
    public class SoftSign : Layer
    {
        private float[] input;
        
        public float[] forward(float[] input)
        {
            this.input = input;
            float[] copy = new float[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                copy[i] = input[i] / (Math.Abs(input[i]) + 1);
            }

            return copy;
        }

        public float[] backward(float[] grads)
        {
            float[] copy = new float[grads.Length];
            for (int i = 0; i < copy.Length; i++)
            {
                copy[i] = 1 / (float) Math.Pow(Math.Abs(input[i]) + 1, 2);
                copy[i] *= grads[i];
            }

            return copy;
        }
    }
}