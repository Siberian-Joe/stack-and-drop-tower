using System.Collections.Generic;

namespace Game.Progress.Runtime
{
    public sealed class TowerProgressSnapshot
    {
        private readonly IReadOnlyList<CubePlacementSnapshot> _cubes;
        public IReadOnlyList<CubePlacementSnapshot> Cubes => _cubes;

        public TowerProgressSnapshot(IReadOnlyList<CubePlacementSnapshot> cubes)
        {
            _cubes = cubes;
        }
    }
}