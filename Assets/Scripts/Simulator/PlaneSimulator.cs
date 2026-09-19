using System;
using System.Collections.Generic;
using System.Linq;
using AI.PostInput;
using Level;
using PlaneInput;
using TMPro;
using UnityEngine;

namespace Simulator
{
    public class PlaneSimulator
    {
        #region Fields

        private IPlaneInput _input;
        private Pose _initialPose;
        private Pose _pose;
        private Level.Level _level;
        private CapsuleCollider _planeCollider;
        private List<Ring>.Enumerator _currentRing;
        private Bounds _currentRingBounds;
        private UpdateRotation _nextRot;
        private UpdatePosition _nextPos;
        private RingLocalizer _ringLoc;
        private List<GameObject> _ringObjects;
        private TMP_Text _scoreCounter;
        private bool _isVisual;
        
        #endregion

        #region Properties
        
        public Pose Pose => _pose;
        public IPlaneInput Input => _input;
        public List<Ring>.Enumerator CurrentRing => _currentRing;
        public bool IsLevelComplete { get; private set; }

        #endregion
        
        #region Constructors

        public PlaneSimulator(IPlaneInput input, Pose pose, float velocity, float yawSpeed, float pitchSpeed,
            float rollSpeed, Level.Level level)
        {
            this._input = input;
            _initialPose = pose;
            this._level = level;

            _nextRot = new UpdateRotation(yawSpeed, pitchSpeed, rollSpeed);
            _nextPos = new UpdatePosition(velocity);
            _ringLoc = new RingLocalizer();

            Reset();
        }

        #endregion

        #region Functions

        public (Vector3, UpdatePosition) CalculateNextPos(Vector3 initial, Quaternion orientation, float thrust)
        {
            return (_nextPos.calculateNextPos(initial, orientation, thrust), _nextPos);
        }
        
        public (Quaternion, UpdateRotation) CalculateNextRot(Quaternion initial, float x, float y, float z)
        {
            return (_nextRot.calculateNextRot(initial, x, y, z), _nextRot);
        }

        private Bounds PlaneBounds()
        {
            return new Bounds(_pose.position + new Vector3(0, 0.6f, -0.2f), new Vector3(1, 1, 2));
        }

        private void UpdateCurrentRingBounds()
        {
            var xRatio = (_currentRing.Current.Pose.rotation * (10f * Vector3.right));
            var yRatio = (_currentRing.Current.Pose.rotation * (10f * Vector3.up));
            var zRatio = (_currentRing.Current.Pose.rotation * (3f * Vector3.forward));
            
            _currentRingBounds = new Bounds(
                _currentRing.Current.Pose.position,
                new Vector3(
                    Math.Abs(xRatio.x) + Math.Abs(yRatio.x) + Math.Abs(zRatio.x), 
                    Math.Abs(xRatio.y) + Math.Abs(yRatio.y) + Math.Abs(zRatio.y), 
                    Math.Abs(xRatio.z) + Math.Abs(yRatio.z) + Math.Abs(zRatio.z))
                
            );
        }

        public void Tick()
        {
            if (_input is AIPlaneInput)
            {
                ((AIPlaneInput)_input).Tick(GetLocalRingPos(_pose.position, _pose.rotation));
            }

            _pose.rotation = CalculateNextRot(_pose.rotation, _input.GetVertical(), _input.GetRudder(),
                _input.GetHorizontal()).Item1;

            _pose.position = CalculateNextPos(_pose.position, _pose.rotation, _input.GetThrust()).Item1;

            // COLLISION DETECTION
            if (IsLevelComplete)
            {
                return;
            }

            if (PlaneBounds().Intersects(_currentRingBounds))
            {
                if (_isVisual)
                {
                    var selectedRing = _ringObjects.FirstOrDefault(
                        r => r.transform.position == _currentRing.Current.Pose.position);
                    selectedRing.GetComponent<Renderer>().material.color = Color.green;   // ovako se mijenja boja prstena 
                }


                if (_currentRing.MoveNext())
                {
                    // Debug.Log("Ring passed! Next: " + currentRing.Current.Pose.position);
                    UpdateCurrentRingBounds();
                    
                    if (_isVisual)
                    {
                        _scoreCounter.text = GetPassedRings() + "/" + _level.Rings.Count;
                        var selectedRing = _ringObjects.FirstOrDefault(
                            r => r.transform.position == _currentRing.Current.Pose.position);
                        selectedRing.GetComponent<Renderer>().material.color = Color.magenta;   // ovako se mijenja boja prstena 
                    }
                }
                else
                {
                    IsLevelComplete = true;
                    Debug.Log("SVI PRSTENI ZAVRSENI!");
                }
            }
        }

        public void ChangeLevel(Level.Level level)
        {
            _level = level;
            Reset();
        }

        public void Reset()
        {
            _pose = _initialPose;
            _currentRing = _level.Rings.GetEnumerator();
            _currentRing.MoveNext();
            UpdateCurrentRingBounds();
        }

        public void UpdateTransform(Transform transform)
        {
            transform.position = _pose.position;
            transform.rotation = _pose.rotation;
        }

        public void SetSceneObjects(List<GameObject> ringObjects, TMP_Text scoreCounter)
        {
            _ringObjects = ringObjects;
            _scoreCounter = scoreCounter;
            scoreCounter.text = "0/" + ringObjects.Count;
            ringObjects[0].GetComponent<Renderer>().material.color = Color.red;   // ovako se mijenja boja prstena 
            _isVisual = true;
        }

        public int GetPassedRings()
        {
            if (_currentRing.Current != null) return _level.Rings.IndexOf(_currentRing.Current);
            IsLevelComplete = true;
            return _level.Rings.Count;
        }

        public int GetRemainingRings()
        {
            if (_currentRing.Current != null) return _level.Rings.Count - GetPassedRings();
            IsLevelComplete = true;
            return 0;
        }

        public Vector3 GetLocalRingPos(Vector3 planePos, Quaternion planeRot)
        {
            return _ringLoc.getLocalRingPos(_currentRing.Current, planePos, planeRot);
        }

        #endregion
    }
}