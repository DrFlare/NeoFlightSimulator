using AI;
using PlaneInput;

namespace Simulator
{
    public class IndependentFlightSimulation
    {
        #region Fields
        
        private bool _isRunning;
        private PlaneSimulator _simulator;
        private int _iter = -1;
        private int _numIter;
        private readonly NeuralNet _net;

        #endregion

        #region Constructors

        public IndependentFlightSimulation(PlaneSimulator simulator)
        {
            _simulator = simulator;
        }

        public IndependentFlightSimulation(PlaneSimulator simulator, int numIter)
        {
            _simulator = simulator;
            _net = ((AIPlaneInput)simulator.Input).Net;
            Reset(numIter);
        }

        #endregion

        #region Functions

        public void StartTestSimulation()
        {
            _isRunning = true;
            if (_numIter > 0)
            {
                for (int i = 0; i < _numIter; i++)
                {
                    Tick();
                    if (_simulator.IsLevelComplete)
                    {
                        break;
                    }
                }
            }
        }

        public void StopTestSimulation()
        {
            _isRunning = false;
        }

        public void Tick()
        {
            if (!_isRunning)
            {
                return;
            }

            _simulator.Tick();

            if (_net != null)
            {
                NetworkStep();
            }

            _iter++;
            if (_numIter > 0 && _iter >= _numIter)
            {
                StopTestSimulation();
            }
        }

        public void Reset(int numIter)
        {
            this._numIter = numIter;
            _iter = 0;
            _simulator.Reset();
        }

        private void NetworkStep()
        {
            var planePos = _simulator.Pose.position;
            var planeRot = _simulator.Pose.rotation;
            var ringPos = _simulator.CurrentRing.Current == null ? planePos : _simulator.CurrentRing.Current.Pose.position;
            var localRingPos = _simulator.GetLocalRingPos(planePos, planeRot);

            float[] packed =
            {
                localRingPos.x, localRingPos.y, localRingPos.z
            };

            var outputs = _net.forward(packed);

            // forward gotov, idemo loss i backward

            var nextPlaneRot = _simulator.CalculateNextRot(planeRot, outputs[0], outputs[1], outputs[2]);
            var nextPlanePos =
                _simulator.CalculateNextPos(planePos, nextPlaneRot.Item1, outputs.Length < 4 ? 0 : outputs[3]);
            
            var loss = _net.loss(planePos, nextPlanePos.Item2, nextPlaneRot.Item2, ringPos, _simulator.GetRemainingRings());

            // Debug.Log("loss = " + loss);

            var dLdXYZ = _net.backward(loss);

            // BACKPROP GOTOV!!!!!!!!!!
            // OPTIM TIME
            _net.optimStep();
        }

        #endregion
    }
}