using System.Collections.Generic;
using UnityEngine;

namespace FlightSimulator
{
    public class Level
    {
        public Level(List<Ring> rings)
        {
            this.Rings = rings;
        }

        public List<Ring> Rings { get; }
    }
}