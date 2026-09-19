using System.Collections.Generic;
using Architecture;
using Controllers;
using Level;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Simulator
{
    public class SimulationController : MonoBehaviour
    {
        #region Serialize Fields

        [SerializeField] private SimulationContext context;
        [SerializeField] private TMP_Text scoreCounter;
        [SerializeField] private PlaneController planeController;
        [SerializeField] private GameObject ring;
        [SerializeField] private List<GameObject> ringObjects = new();

        #endregion

        #region Functions

        private static void EndSimulation()
        {
            SceneManager.LoadScene(Constants.MainMenuSceneName);
        }

        #endregion
        
        #region Unity Functions

        private void Start()
        {
            var level = LevelLoader.LoadLevel(context.levelName);

            foreach (var ringPos in level.Rings)
            {
                ringObjects.Add(Instantiate(ring, ringPos.Pose.position, ringPos.Pose.rotation));
            }

            planeController.Sim.SetSceneObjects(ringObjects, scoreCounter);
            new IndependentFlightSimulation(planeController.Sim).StartTestSimulation();
        }

        private void Update()
        {
            // TODO: Transfer input handling to a unified input controller
            if (Input.GetKeyDown(KeyCode.Escape) || planeController.Sim.IsLevelComplete)
                EndSimulation();
        }

        #endregion
    }
}