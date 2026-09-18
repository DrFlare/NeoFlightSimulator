using System.Collections.Generic;

namespace Level
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