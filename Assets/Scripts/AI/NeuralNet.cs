using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FlightSimulator.AI
{
    public class NeuralNet
    {
        private const float minWeight = -0.1f;
        private const float maxWeight = 0.1f;

        private const float rotDiffFactor = 0.3f;
        private Vector3 punishmentFactors = new Vector3(0.1f, 0.1f, 0.2f);
        private const float lr = 0.001f;

        private static readonly int[] hiddenLayers = { 10 };

        private List<int> dimensions;
        private List<FCLayer> layers;
        private List<Layer> activationFuncs;

        private UpdatePosition nextPos;
        private UpdateRotation nextRot;

        private Vector3 ringPos;
        private Vector3 nextRingPos;

        private float[] output;
        private DistDiff distDiff;
        private RotDiff rotDiff;

        private float lastLoss = -1;
        private float lowestLoss = -1;

        public NeuralNet()
        {
            // skriveni slojevi
            dimensions = new List<int>(hiddenLayers);
            // ulazni sloj
            dimensions.Insert(0, 3);
            // izlazni sloj
            dimensions.Add(4);

            activationFuncs = new List<Layer>();

            for (var i = 0; i < dimensions.Count - 1; i++)
            {
                activationFuncs.Add(new SoftSign());
            }

            layers = new List<FCLayer>();

            var effectiveWeightSpan = maxWeight / 10 - minWeight / 10;
            for (int d = 0; d < dimensions.Count - 1; d++)
            {
                var tempLayer = new FCLayer(dimensions[d], dimensions[d + 1], effectiveWeightSpan);

                // stavljanje rezina na normalne - prvi sloj je popunjen
                if (d == 0)
                {
                    effectiveWeightSpan = maxWeight - minWeight;
                }

                layers.Add(tempLayer);
            }

            // saveWeights("initial");

            rotDiff = new RotDiff();
            distDiff = new DistDiff();
        }

        public NeuralNet(string weightsPath)
        {
            loadWeights(weightsPath);
            rotDiff = new RotDiff();
            distDiff = new DistDiff();
        }

        public float[] forward(float[] inputs)
        {
            // Debug.Log("ULAZ U MREZU= " + string.Join(", ", inputs));

            var h = inputs;

            for (int i = 0; i < layers.Count; i++)
            {
                h = layers[i].forward(h);
                h = activationFuncs[i].forward(h);
            }

            output = h;
            return h;
        }

        public float[] backward(float loss)
        {
            var grads = distDiff.backward(new[] { loss });
            var thrustGrad = nextPos.backwardThrust(grads);
            grads = nextPos.backward(grads);

            var grads2 = rotDiff.backward(new[] { loss * rotDiffFactor });
            for (int i = 0; i < 4; i++)
            {
                grads[i] += grads2[i];
            }

            grads = nextRot.backward(grads);


            // var outputVector = new Vector3(output[0], output[1], output[2]);
            // for (int i = 0; i < grads.Length; i++)
            // {
            //     grads[i] += punishmentFactors[i] * (Math.Sign(output[i]));
            // }

            grads = new[] {grads[0], grads[1], grads[2], thrustGrad};

            for (int i = layers.Count - 1; i >= 0; i--)
            {
                grads = activationFuncs[i].backward(grads);
                // Debug.Log("dLdTF[" + i + "]= " + string.Join(", ", grads));
                layers[i].backwardWeights(grads);
                layers[i].backwardBiases(grads);
                grads = layers[i].backward(grads);
                // Debug.Log("dLdFCN[" + i + "]= " + string.Join(", ", grads));
            }

            return grads; // ne znam zasto vracam gradijente po ulazu hehe
        }

        public float loss(Vector3 planePos, UpdatePosition nextPlanePos, UpdateRotation nextPlaneRot, Vector3 ringPos,
            float remainingRings)
        {
            var dd = distDiff.calculateDistDiff(planePos, nextPlanePos, ringPos);

            var rd = rotDiff.calculateRotDiff(planePos, ringPos, nextPlaneRot);

            // this.ringPos = ringPos;

            nextPos = nextPlanePos;
            nextRot = nextPlaneRot;

            // var outputVector = new Vector3(Math.Abs(output[0]), Math.Abs(output[1]), Math.Abs(output[2]));
            
            // var loss = dd + Vector3.Dot(punishmentFactors, outputVector); // rotDiff trenutacno onemogucen
            var loss = dd + rotDiffFactor * rd + remainingRings;

            lastLoss = loss;

            if (lowestLoss < 0 || loss < lowestLoss)
            {
                lowestLoss = loss;
            }

            return loss;
        }

        public void optimStep()
        {
            foreach (var layer in layers)
            {
                layer.optimStep(lr);
            }
        }

        public void updateBestWeights()
        {
            foreach (var layer in layers)
            {
                layer.updateBestWeights();
            }
        }

        public string saveWeights(string prefix)
        {
            var path = DateTime.Now.ToString("dd-MM-HH-mm") + "-" + prefix;

            var writer =
                File.AppendText("Assets/NeuralNet/weights/" + path + ".txt");


            writer.WriteLine(String.Join("x", dimensions));

            foreach (var line in layers.Select(layer => layer.exportWeights()).SelectMany(weights => weights))
            {
                writer.WriteLine(line);
            }

            writer.Flush();
            writer.Close();

            return path;
        }

        public string saveBestWeights(string prefix)
        {
            var path = DateTime.Now.ToString("dd-MM-HH-mm") + "-" + prefix;

            var writer =
                File.AppendText("Assets/NeuralNet/weights/" + path + ".txt");

            writer.WriteLine(String.Join("x", dimensions));

            foreach (var line in layers.Select(layer => layer.exportBestWeights()).SelectMany(weights => weights))
            {
                writer.WriteLine(line);
            }
            
            writer.Flush();
            writer.Close();

            return path;
        }

        public void loadWeights(string path)
        {
            var parameters = new List<string>(File.ReadAllLines("Assets/NeuralNet/weights/" + path + ".txt"));

            var dims = parameters[0].Split("x").Select(int.Parse).ToList();

            activationFuncs = new List<Layer>();

            for (var i = 0; i < dims.Count - 1; i++)
            {
                activationFuncs.Add(new SoftSign());
            }

            layers = new List<FCLayer>();

            var idx = 1;
            FCLayer layer;
            for (int d = 0; d < dims.Count - 1; d++)
            {
                layer = new FCLayer(dims[d], dims[d + 1], parameters.GetRange(idx, dims[d] + 1));
                idx += dims[d] + 1;

                layers.Add(layer);
            }
        }

        public float LastLoss => lastLoss;

        public float LowestLoss => lowestLoss;

        public static List<string> getSavedWeights()
        {
            var ret = new List<string>();

            foreach (var file in Directory.GetFiles("Assets/NeuralNet/weights").Reverse())
            {
                if (!file.Contains(".meta"))
                {
                    ret.Add(Regex.Replace(file, ".*(/|\\\\)", "").Replace(".txt", ""));
                }
            }

            return ret;
        }
    }
}