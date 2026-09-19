using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using PlaneInput;
using Simulator;
using UnityEngine;
using UnityEngine.Assertions;

namespace Level
{
    public static class LevelLoader
    {
        #region Constants
        
        private const float InitialVelocity = 2f;
        private const float YawSpeed = 7f;
        private const float PitchSpeed = 15f;
        private const float RollSpeed = 15f;

        #endregion
        
        #region Fields

        private static List<string> _levels;

        #endregion

        #region Functions

        private static void GenerateFromString(out Pose pose, string data)
        {
            var split = data.Split(";");
            Assert.AreEqual(split.Length, 2);

            var pos = split[0].Split(" ");
            Assert.AreEqual(pos.Length, 3);

            var position = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
            pose.position = position;

            var rot = split[1].Split(" ");
            Assert.AreEqual(rot.Length, 3);

            pose.rotation = Quaternion.Euler(
                new Vector3(float.Parse(rot[0]), float.Parse(rot[1]), float.Parse(rot[2]))
            );
        }

        public static Level LoadLevel(string name)
        {
            var levelPath = "Assets/Levels/" + name + ".txt";
            // MonoBehaviour.print(levelPath);
            var lines = File.ReadAllLines(levelPath);

            var rings = new List<Ring>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                GenerateFromString(out var pose, line);

                rings.Add(new Ring(pose));
            }

            return new Level(rings);
        }

        public static List<string> GetLevelNames()
        {
            if (_levels != null) return _levels;
            _levels = new List<string>();
            foreach (var file in Directory.GetFiles("Assets/levels/"))
            {
                if (!file.Contains(".meta"))
                {
                    _levels.Add(Regex.Replace(file, ".*/", "").Replace(".txt", ""));
                }
            }

            return _levels;
        }

        public static PlaneSimulator DummyPlaneSimulator(Level level, AIPlaneInput input)
        {
            return new PlaneSimulator(input, new Pose(Vector3.zero, Quaternion.identity), 
                InitialVelocity, YawSpeed, PitchSpeed, RollSpeed, level);
        }

        #endregion
    }
}