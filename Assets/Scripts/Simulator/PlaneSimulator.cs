using System;
using System.Collections.Generic;
using System.Linq;
using FlightSimulator.AI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace FlightSimulator
{
    public class PlaneSimulator
    {
        private PlaneInput input;

        private Pose initialPose;

        private Pose pose;

        private Level level;

        private CapsuleCollider planeCollider;

        private List<Ring>.Enumerator currentRing;
        private Bounds currentRingBounds;

        private UpdateRotation nextRot;
        private UpdatePosition nextPos;
        private RingLocalizer ringLoc;

        private List<GameObject> ringObjects;
        private TMP_Text scoreCounter;

        private bool visual = false;
        
        public PlaneSimulator(PlaneInput input, Pose pose, float velocity, float yawSpeed, float pitchSpeed,
            float rollSpeed, Level level)
        {
            this.input = input;
            initialPose = pose;
            this.level = level;

            nextRot = new UpdateRotation(yawSpeed, pitchSpeed, rollSpeed);
            nextPos = new UpdatePosition(velocity);
            ringLoc = new RingLocalizer();

            reset();
        }
        
        public (Vector3, UpdatePosition) calculateNextPos(Vector3 initial, Quaternion orientation, float thrust)
        {
            return (nextPos.calculateNextPos(initial, orientation, thrust), nextPos);
        }


        public (Quaternion, UpdateRotation) calculateNextRot(Quaternion initial, float x, float y, float z)
        {
            return (nextRot.calculateNextRot(initial, x, y, z), nextRot);
        }

        private Bounds planeBounds()
        {
            return new Bounds(pose.position + new Vector3(0, 0.6f, -0.2f), new Vector3(1, 1, 2));
        }

        private void updateCurrentRingBounds()
        {
            var xRatio = (currentRing.Current.Pose.rotation * (10f * Vector3.right));
            var yRatio = (currentRing.Current.Pose.rotation * (10f * Vector3.up));
            var zRatio = (currentRing.Current.Pose.rotation * (3f * Vector3.forward));
            
            currentRingBounds = new Bounds(
                currentRing.Current.Pose.position,
                new Vector3(
                    Math.Abs(xRatio.x) + Math.Abs(yRatio.x) + Math.Abs(zRatio.x), 
                    Math.Abs(xRatio.y) + Math.Abs(yRatio.y) + Math.Abs(zRatio.y), 
                    Math.Abs(xRatio.z) + Math.Abs(yRatio.z) + Math.Abs(zRatio.z))
                
            );
        }

        public void tick()
        {
            if (input is AIPlaneInput)
            {
                ((AIPlaneInput)input).tick(getLocalRingPos(pose.position, pose.rotation));
            }

            pose.rotation = calculateNextRot(pose.rotation, input.getVertical(), input.getRudder(),
                input.getHorizontal()).Item1;

            pose.position = calculateNextPos(pose.position, pose.rotation, input.getThrust()).Item1;

            // COLLISION DETECTION
            if (LevelComplete)
            {
                return;
            }

            if (planeBounds().Intersects(currentRingBounds))
            {
                if (visual)
                {
                    var selectedRing = ringObjects.FirstOrDefault(r => r.transform.position == currentRing.Current.Pose.position);
                    selectedRing.GetComponent<Renderer>().material.color = Color.green;   // ovako se mijenja boja prstena 
                }


                if (currentRing.MoveNext())
                {
                    Debug.Log("prsten rijesen! sljedeci je na " + currentRing.Current.Pose.position);
                    updateCurrentRingBounds();
                    
                    if (visual)
                    {
                        scoreCounter.text = getPasseedRings() + "/" + level.Rings.Count;
                        var selectedRing = ringObjects.FirstOrDefault(r => r.transform.position == currentRing.Current.Pose.position);
                        selectedRing.GetComponent<Renderer>().material.color = Color.magenta;   // ovako se mijenja boja prstena 
                    }
                }
                else
                {
                    LevelComplete = true;
                    Debug.Log("SVI PRSTENI ZAVRSENI!");
                }
            }
        }

        public void changeLevel(Level level)
        {
            this.level = level;
            reset();
        }

        public void reset()
        {
            pose = initialPose;
            currentRing = level.Rings.GetEnumerator();
            currentRing.MoveNext();
            updateCurrentRingBounds();
        }

        public void updateTransform(Transform transform)
        {
            transform.position = pose.position;
            transform.rotation = pose.rotation;
        }

        public void setSceneObjects(List<GameObject> ringObjects, TMP_Text scoreCounter)
        {
            this.ringObjects = ringObjects;
            this.scoreCounter = scoreCounter;
            scoreCounter.text = "0/" + ringObjects.Count;
            ringObjects[0].GetComponent<Renderer>().material.color = Color.red;   // ovako se mijenja boja prstena 
            visual = true;
        }
        
        public Pose Pose => pose;

        public PlaneInput Input => input;

        public List<Ring>.Enumerator CurrentRing => currentRing;

        public bool LevelComplete { get; private set; } = false;

        public int getPasseedRings()
        {
            if (currentRing.Current != null) return level.Rings.IndexOf(currentRing.Current);
            LevelComplete = true;
            return level.Rings.Count;
        }

        public int getRemainingRings()
        {
            if (currentRing.Current != null) return level.Rings.Count - getPasseedRings();
            LevelComplete = true;
            return 0;
        }

        public Vector3 getLocalRingPos(Vector3 planePos, Quaternion planeRot)
        {
            return ringLoc.getLocalRingPos(currentRing.Current, planePos, planeRot);
        }
    }
}