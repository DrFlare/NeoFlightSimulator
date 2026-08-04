using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;

namespace FlightSimulator
{
    public static class LevelLoader
    {
        private static List<string> levels;

        private static float initialVelocity = 2f;

        private static float yawSpeed = 7f;

        private static float pitchSpeed = 15f;

        private static float rollSpeed = 15f;

        private static void generateFromString(out Pose pose, string data)
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

        public static Level loadLevel(string name)
        {
            string levelPath = "Assets/Levels/" + name + ".txt";
            // MonoBehaviour.print(levelPath);
            var lines = File.ReadAllLines(levelPath);

            List<Ring> rings = new List<Ring>();

            foreach (var line in lines)
            {
                if (String.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                Pose pose = new Pose();
                generateFromString(out pose, line);

                rings.Add(new Ring(pose));
            }

            return new Level(rings);
        }

        public static List<string> getLevelNames()
        {
            if (levels == null)
            {
                levels = new List<string>();
                foreach (var file in Directory.GetFiles("Assets/levels/"))
                {
                    if (!file.Contains(".meta"))
                    {
                        levels.Add(Regex.Replace(file, ".*/", "").Replace(".txt", ""));
                    }
                }
            }

            return levels;
        }

        public static PlaneSimulator DummyPlaneSimulator(Level level, AIPlaneInput input)
        {
            return new PlaneSimulator(input, new Pose(Vector3.zero, Quaternion.identity), initialVelocity, yawSpeed, pitchSpeed, rollSpeed, level);
        }
    }
}