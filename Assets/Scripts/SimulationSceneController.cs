using System.Collections.Generic;
using System.Threading;
using FlightSimulator;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulationSceneController : MonoBehaviour
{
    public SimulationContext context;

    public TMP_Text scoreCounter;
    public PlaneController planeController;
    public GameObject ring;

    public List<GameObject> ringObjects = new();
    public bool initialized = false;

    // Start is called before the first frame update
    void Start()
    {
        Level level = LevelLoader.loadLevel(context.levelName);

        foreach (var ringPos in level.Rings)
        {
            ringObjects.Add(Instantiate(ring, ringPos.Pose.position, ringPos.Pose.rotation));
        }
        
        initialize();
        new IndependentFlightSimulation(planeController.Sim).startTestSimulation();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!initialized)
        {
            initialize();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape) || planeController.Sim.LevelComplete)
        {
            SceneManager.LoadScene("MenuScene");
        }
    }

    private void initialize()
    {
        if (planeController.Sim != null)
        {
            planeController.Sim.setSceneObjects(ringObjects, scoreCounter);
            initialized = true;
        }
    }
}