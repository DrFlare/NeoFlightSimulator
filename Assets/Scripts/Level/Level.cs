using System.Collections.Generic;

namespace Level
{
    public class Level
    {
        public List<Ring> Rings { get; }
        
        public Level(List<Ring> rings)
        {
            Rings = rings;
        }
    }
}