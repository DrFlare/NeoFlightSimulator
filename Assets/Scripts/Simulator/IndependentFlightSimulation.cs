using FlightSimulator.AI;
using UnityEngine;

namespace FlightSimulator
{
    public class IndependentFlightSimulation
    {
        private bool running = false;

        private PlaneSimulator simulator;

        private int iter = -1;

        private int numIter;

        private NeuralNet net = null;

        public IndependentFlightSimulation(PlaneSimulator simulator)
        {
            this.simulator = simulator;
        }

        public IndependentFlightSimulation(PlaneSimulator simulator, int numIter)
        {
            this.simulator = simulator;
            net = ((AIPlaneInput)simulator.Input).Net;
            reset(numIter);
        }

        public void startTestSimulation()
        {
            running = true;
            if (numIter > 0)
            {
                for (int i = 0; i < numIter; i++)
                {
                    tick();
                    if (simulator.LevelComplete)
                    {
                        break;
                    }
                }
            }
        }

        public void stopTestSimulation()
        {
            running = false;
        }

        public void tick()
        {
            if (!running)
            {
                return;
            }

            simulator.tick();

            if (net != null)
            {
                networkStep();
            }

            iter++;
            if (numIter > 0 && iter >= numIter)
            {
                stopTestSimulation();
            }
        }

        public void reset(int numIter)
        {
            this.numIter = numIter;
            iter = 0;
            simulator.reset();
        }

        private void networkStep()
        {
            var planePos = simulator.Pose.position;
            var planeRot = simulator.Pose.rotation;
            var ringPos = simulator.CurrentRing.Current == null ? planePos : simulator.CurrentRing.Current.Pose.position;
            var localRingPos = simulator.getLocalRingPos(planePos, planeRot);

            float[] packed =
            {
                localRingPos.x, localRingPos.y, localRingPos.z
            };

            var outputs = net.forward(packed);

            // forward gotov, idemo loss i backward

            var nextPlaneRot = simulator.calculateNextRot(planeRot, outputs[0], outputs[1], outputs[2]);
            var nextPlanePos =
                simulator.calculateNextPos(planePos, nextPlaneRot.Item1, outputs.Length < 4 ? 0 : outputs[3]);
            
            var loss = net.loss(planePos, nextPlanePos.Item2, nextPlaneRot.Item2, ringPos, simulator.getRemainingRings());

            // Debug.Log("loss = " + loss);

            var dLdXYZ = net.backward(loss);

            // BACKPROP GOTOV!!!!!!!!!!
            // OPTIM TIME
            net.optimStep();
        }
    }
}