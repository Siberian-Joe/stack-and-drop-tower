using System;

namespace Game.Progress.Data
{
    [Serializable]
    public sealed class GameProgressData
    {
        public int SchemaVersion = 1;

        public TowerProgressData Tower = new();
        public LevelProgressData Levels = new();
    }
}