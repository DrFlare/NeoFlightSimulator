using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Level
{
    public static class LevelGenerator
    {
        public static void GenerateLevels(int levelCount, int ringDist, int ringDistVariance, 
            float maxAngle, int ringCount, string prefix)
        {
            var sb = new StringBuilder();
            var lines = new List<string>();

            for (var i = 0; i < levelCount; i++)
            {
                var position = Vector3.zero;
                var rotation = Quaternion.identity;
                lines.Clear();
                
                for (var j = 0; j < ringCount; j++)
                {
                    sb.Clear();

                    position += rotation * (Vector3.forward * 
                                            Random.Range(ringDist - ringDistVariance, ringDist + ringDistVariance));
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