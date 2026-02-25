using System;
using System.Collections.Generic;

namespace Game.Progress.Runtime
{
    public readonly struct TowerProgressSnapshot
    {
        public IReadOnlyList<CubePlacementSnapshot> Cubes { get; }

        public TowerProgressSnapshot(IReadOnlyList<CubePlacementSnapshot> cubes) =>
            Cubes = cubes ?? Array.Empty<CubePlacementSnapshot>();
    }
}