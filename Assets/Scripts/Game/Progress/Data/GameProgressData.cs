using System;

namespace Game.Progress.Data
{
    [Serializable]
    public sealed class GameProgressData
    {
        public int schemaVersion = 1;

        public TowerProgressData tower = new();
        public LevelProgressData levels = new();
    }
}