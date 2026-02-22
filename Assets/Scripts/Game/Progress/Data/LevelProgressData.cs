using System;

namespace Game.Progress.Data
{
    [Serializable]
    public sealed class LevelProgressData
    {
        public int completedLevels;
        public int currentLevelIndex;
    }
}