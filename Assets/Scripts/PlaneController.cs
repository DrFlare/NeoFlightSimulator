using System;
using FlightSimulator;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    public SimulationContext context;

    private PlaneInput input;

    private const int VELOCITY_SCALE = 10;
    private const int ANGLE_SCALE = 10;

    public GameObject propeller;

    public float initialVelocity = 0.1f;
    
    public float yawSpeed;
        
    public float pitchSpeed;
        
    public float rollSpeed;

    void Start()
    {
        input = context.inputType switch
        {
            InputType.Human => new HumanPlaneInput(),
            InputType.AI => string.IsNullOrEmpty(context.neuralNetWeightsPath) ? new AIPlaneInput() : new AIPlaneInput(context.neuralNetWeightsPath),
            _ => throw new ArgumentException()
        };

        Level level = LevelLoader.loadLevel(context.levelName);

        var pose = new Pose(gameObject.transform.position, gameObject.transform.rotation);
        
        Sim = new PlaneSimulator(input, pose, (initialVelocity / VELOCITY_SCALE), yawSpeed / ANGLE_SCALE, pitchSpeed / ANGLE_SCALE, rollSpeed / ANGLE_SCALE, level);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Sim.tick();
        Sim.updateTransform(transform);
        propeller.transform.Rotate(Vector3.right, 20);
    }

    public PlaneSimulator Sim { get; private set; }
}
