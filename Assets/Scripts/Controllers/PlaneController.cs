using System;
using Level;
using PlaneInput;
using Simulator;
using UnityEngine;

namespace Controllers
{
    public class PlaneController : MonoBehaviour
    {
        #region Constants

        private const int VelocityScale = 10;
        private const int AngleScale = 10;

        #endregion
        
        #region Fields
        
        private IPlaneInput _input;

        #endregion

        #region Serialize Fields
        
        [SerializeField] private SimulationContext context;
        [SerializeField] private GameObject propeller;
        [SerializeField] private float initialVelocity = 0.1f;
        [SerializeField] private float yawSpeed;
        [SerializeField] private float pitchSpeed;
        [SerializeField] private float rollSpeed;

        #endregion

        #region Properties
        
        public PlaneSimulator Sim { get; private set; }

        #endregion

        #region Unity Functions

        private void Start()
        {
            _input = context.inputType switch
            {
                InputType.Human => new HumanPlaneInput(),
                InputType.AI => string.IsNullOrEmpty(context.neuralNetWeightsPath) ? new AIPlaneInput() : new AIPlaneInput(context.neuralNetWeightsPath),
                _ => throw new ArgumentException()
            };

            Level.Level level = LevelLoader.LoadLevel(context.levelName);

            var pose = new Pose(gameObject.transform.position, gameObject.transform.rotation);
        
            Sim = new PlaneSimulator(_input, pose, (initialVelocity / VelocityScale), yawSpeed / AngleScale, pitchSpeed / AngleScale, rollSpeed / AngleScale, level);
        }

        private void FixedUpdate()
        {
            Sim.Tick();
            Sim.UpdateTransform(transform);
            propeller.transform.Rotate(Vector3.right, 20);
        }

        #endregion
    }
}
