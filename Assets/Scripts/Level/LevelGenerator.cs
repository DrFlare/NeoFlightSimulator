using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace FlightSimulator
{
    public static class LevelGenerator
    {
        public static void generateLevels(int levelCount, int ringDist, int ringDistVariance, float maxAngle, int ringCount, string prefix)
        {
            Vector3 position;
            Quaternion rotation;
            StringBuilder sb = new StringBuilder();
            var lines = new List<string>();

            for (int i = 0; i < levelCount; i++)
            {
                position = Vector3.zero;
                rotation = Quaternion.identity;
                lines.Clear();
                
                for (int j = 0; j < ringCount; j++)
                {
                    sb.Clear();

                    position += rotation * (Vector3.forward * Random.Range(ringDist - ringDistVariance, ringDist + ringDistVariance));
                    if (j > 0)
                    {
                        var randomRotation = Random.rotationUniform;
                        randomRotation = Quaternion.RotateTowards(Quaternion.identity, randomRotation, maxAngle);

                        rotation *= randomRotation;
                        
                        rotation = rotation.normalized;
                    }

                    sb.Append(position.x)
                        .Append(" ")
                        .Append(position.y)
                        .Append(" ")
                        .Append(position.z)
                        .Append(";");

                    var euler = rotation.eulerAngles;

                    sb.Append(euler.x).Append(" ").Append(euler.y).Append(" ").Append(euler.z);
                    
                    lines.Add(sb.ToString());
                }
                File.WriteAllLines("Assets/Levels/" + prefix + (i + 1) + ".txt", lines);
            }
        }
    }
}