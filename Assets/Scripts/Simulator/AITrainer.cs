using Level;
using PlaneInput;
using UnityEngine;

namespace Simulator
{
    public class AITrainer
    {
        #region Fields

        private int _epochNo = 200; // fallback
        private int _frameNo = 100; // fallback
        private AIPlaneInput _input = new();
        private IndependentFlightSimulation _flightSimulation;
        private PlaneSimulator _sim;
        private float _lowestTrainLoss = float.MaxValue;
        private int _highestPassedRings = 0;

        #endregion

        public void StartTrainingSimulation(int numEpochs, int numFrames)
        {
            var random = new System.Random();
        
            _epochNo = numEpochs;
            _frameNo = numFrames;

            var list = LevelLoader.GetLevelNames();

            _highestPassedRings = int.MinValue;
            _lowestTrainLoss = float.MaxValue;
            _sim = LevelLoader.DummyPlaneSimulator(LevelLoader.LoadLevel(list[0]), _input);
        
            _flightSimulation =
                new IndependentFlightSimulation(_sim, _frameNo);

            for (var i = 0; i < _epochNo; i++)
            {
                var next = random.Next(list.Count);
                Debug.Log("Next stage: " + next);
                _sim.ChangeLevel(LevelLoader.LoadLevel(list[next]));
                _flightSimulation.Reset(_frameNo * (1 + 5 * i / _epochNo));
                _flightSimulation.StartTestSimulation();

                if (_sim.GetPassedRings() > _highestPassedRings)
                {
                    _highestPassedRings = _sim.GetPassedRings();
                    _input.Net.updateBestWeights();
                    // za spremanje svake granice pri otkrivanju najboljeg rjesenja:
                    _input.Net.saveWeights("PASSES_" + _sim.GetPassedRings()); 
                }

                if (_input.Net.LowestLoss < _lowestTrainLoss)
                {
                    _lowestTrainLoss = _input.Net.LowestLoss;
                    if (_sim.GetPassedRings() == _highestPassedRings)
                    {
                        _input.Net.updateBestWeights();
                    }
                }


                Debug.Log(
                    "Epoch " + i +
                    ": passed rings = " + _sim.GetPassedRings() +
                    ", most passed rings = " + _highestPassedRings +
                    ", last loss = " + _input.Net.LastLoss +
                    ", lowest epoch loss = " + _input.Net.LowestLoss +
                    ", lowest train loss = " + _lowestTrainLoss);

            }


            _input.Net.saveWeights("latest");
            _input.Net.saveBestWeights("best");
            Debug.Log("TRAINING COMPLETE");
            _flightSimulation.StopTestSimulation();
        }
    }
}