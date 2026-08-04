using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Assertions;

namespace FlightSimulator.AI
{
    public class FCLayer : Layer
    {
        private int inputSize;
        private int outputSize;
        private float[][] weights;
        private List<float> bias;
        
        private float[][] bestWeights;
        private List<float> bestBias;

        private float[] inputs;
        private float[] outputs;

        private float[] inputGrads;
        private float[][] weightGrads;
        private float[] biasGrads;

        public FCLayer(int inputSize, int outputSize, float weightSpan)
        {
            var random = new System.Random();
            
            this.inputSize = inputSize;
            this.outputSize = outputSize;

            inputGrads = new float[inputSize];
            weightGrads = new float[inputSize][];
            biasGrads = new float[outputSize];

            weights = new float[inputSize][];
            bias = new List<float>();
            for (int i = 0; i < inputSize; i++)
            {
                weights[i] = new float[outputSize];
                for (int j = 0; j < outputSize; j++)
                {
                    weights[i][j] =
                        (float)(random.NextDouble() * weightSpan - (weightSpan / 2));

                    if (i == 0)
                    {
                        bias.Add((float)(random.NextDouble() * weightSpan -
                                                   (weightSpan / 2)));
                    }
                }
                weightGrads[i] = new float[outputSize];
            }
        }

        public FCLayer(int inputSize, int outputSize, List<string> importedLines)
        {
            importWeights(inputSize, outputSize, importedLines);
        }

        public int InputSize
        {
            get => inputSize;
            set => inputSize = value;
        }

        public int OutputSize
        {
            get => outputSize;
            set => outputSize = value;
        }

        public float[][] Weights
        {
            get => weights;
            set => weights = value;
        }

        public List<float> Bias
        {
            get => bias;
            set => bias = value;
        }

        public float[] forward(float[] inputs)
        {
            this.inputs = inputs;
            outputs = new float[outputSize];
            for (int o = 0; o < outputSize; o++)
            {
                outputs[o] = 0f;
                for (int i = 0; i < inputSize; i++)
                {
                    outputs[o] += inputs[i] * weights[i][o];
                    // outputs[o] = Vector.Dot(new Vector<float>(inputs), new Vector<float>(weights[o])) + bias[o];
                }

                outputs[o] += bias[o];
            }

            return outputs;
        }

        public float[] backward(float[] grads)
        {
            // ne koristiti, koristi ostale
            return backwardInputs(grads);
        }

        public float[] backwardInputs(float[] grads)
        {
            // grads su 3x1 ?
            for (int i = 0; i < inputSize; i++)
            {
                inputGrads[i] = 0f;
                // inputGrads[i] = Vector.Dot(new Vector<float>(grads), new Vector<float>(weights[i]));
                // ^^^ ne moze jer min dimenzija vektora je 3

                for (int o = 0; o < outputSize; o++)
                {
                    inputGrads[i] += grads[o] * weights[i][o];
                }
            }

            return inputGrads;
        }

        public float[][] backwardWeights(float[] grads)
        {
            for (int i = 0; i < inputSize; i++)
            {
                for (int o = 0; o < outputSize; o++)
                {
                    weightGrads[i][o] = grads[o] * inputs[i];
                }
            }
            return weightGrads;
        }

        public float[] backwardBiases(float[] grads)
        {
            biasGrads = grads;
            return grads;
        }

        public void optimStep(float lr)
        {
            for (int i = 0; i < inputSize; i++)
            {
                for (int o = 0; o < outputSize; o++)
                {
                    weights[i][o] -= lr * weightGrads[i][o];
                }
            }

            for (int o = 0; o < outputSize; o++)
            {
                bias[o] -= lr * biasGrads[o];
            }
        }

        public void updateBestWeights()
        {
            bestWeights = weights;
            bestBias = bias;
        }

        public List<string> exportWeights()
        {
            var ret = new List<string>();

            for (int i = 0; i < inputSize; i++)
            {
                ret.Add(string.Join(", ", Weights[i]));
            }

            ret.Add(string.Join(", ", bias));

            return ret;
        }
        
        public List<string> exportBestWeights()
        {
            var ret = new List<string>();

            for (int i = 0; i < inputSize; i++)
            {
                ret.Add(string.Join(", ", bestWeights[i]));
            }

            ret.Add(string.Join(", ", bestBias));

            return ret;
        }

        public void importWeights(int inputSize, int outputSize,  List<string> lines)
        {
            string[] split;

            this.inputSize = inputSize;
            this.outputSize = outputSize;

            weights ??= new float[inputSize][];

            for (int i = 0; i < inputSize; i++)
            {
                split = lines[i].Split(", ");
                weights[i] = Array.ConvertAll(split, float.Parse);
            }

            bias = new List<float>(Array.ConvertAll(lines[inputSize].Split(", "), float.Parse));

            updateBestWeights();
        }
    }
}