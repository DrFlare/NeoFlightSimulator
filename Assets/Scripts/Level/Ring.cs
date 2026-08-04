using UnityEngine;

namespace FlightSimulator
{
    public class Ring
    {
        public Ring(Pose pose)
        {
            this.Pose = pose;
        }

        public Pose Pose { get; }
    }
}