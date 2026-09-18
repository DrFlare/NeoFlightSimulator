using PlaneInput;
using UnityEngine;

namespace Simulator
{
    [CreateAssetMenu(fileName = "SimulationContext", menuName = "ScriptableObjects/SimulationContext", order = 1)]
    public class SimulationContext : ScriptableObject
    {
        public string levelName;
        public InputType inputType;
        public string neuralNetWeightsPath;
    }
}