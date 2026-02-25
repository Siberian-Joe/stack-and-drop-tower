using System;
using System.Collections.Generic;

namespace Game.Progress.Data
{
    [Serializable]
    public sealed class TowerProgressData
    {
        public List<CubePlacementData> Cubes = new();
    }
}