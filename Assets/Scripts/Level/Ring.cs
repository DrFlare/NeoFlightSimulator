using UnityEngine;

namespace Level
{
    public class Ring
    {
        public Ring(Pose pose)
        {
            Pose = pose;
        }

        public Pose Pose { get; }
    }
}