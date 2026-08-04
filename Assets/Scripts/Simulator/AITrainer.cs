using System.Linq;
using FlightSimulator;
using UnityEngine;

public class AITrainer
{
    private int epochNo = 200; // fallback
    private int frameNo = 100; // fallback

    private AIPlaneInput input = new();

    private IndependentFlightSimulation flightSimulation;

    private PlaneSimulator sim;

    private float lowestTrainLoss = float.MaxValue;
    private int highestPassedRings = 0;

    public void startTrainingSimulation(int numEpochs, int numFrames)
    {
        var random = new System.Random();
        
        epochNo = numEpochs;
        frameNo = numFrames;

        var list = LevelLoader.getLevelNames();

        highestPassedRings = int.MinValue;
        lowestTrainLoss = float.MaxValue;
        sim = LevelLoader.DummyPlaneSimulator(LevelLoader.loadLevel(list[0]), input);
        
        flightSimulation =
            new IndependentFlightSimulation(sim, frameNo);

        for (var i = 0; i < epochNo; i++)
        {
            var next = random.Next(list.Count);
            Debug.Log("Next stage: " + next);
            sim.changeLevel(LevelLoader.loadLevel(list[next]));
            flightSimulation.reset(frameNo * (1 + 5 * i / epochNo));
            flightSimulation.startTestSimulation();

            if (sim.getPasseedRings() > highestPassedRings)
            {
                highestPassedRings = sim.getPasseedRings();
                input.Net.updateBestWeights();
                // za spremanje svake granice pri otkrivanju najboljeg rjesenja:
                input.Net.saveWeights("PASSES_" + sim.getPasseedRings()); 
            }

            if (input.Net.LowestLoss < lowestTrainLoss)
            {
                lowestTrainLoss = input.Net.LowestLoss;
                if (sim.getPasseedRings() == highestPassedRings)
                {
                    input.Net.updateBestWeights();
                }
            }


            Debug.Log(
                "Epoch " + i +
                ": passed rings = " + sim.getPasseedRings() +
                ", most passed rings = " + highestPassedRings +
                ", last loss = " + input.Net.LastLoss +
                ", lowest epoch loss = " + input.Net.LowestLoss +
                ", lowest train loss = " + lowestTrainLoss);

        }


        input.Net.saveWeights("latest");
        input.Net.saveBestWeights("best");
        Debug.Log("TRAINING COMPLETE");
        flightSimulation.stopTestSimulation();
    }
}