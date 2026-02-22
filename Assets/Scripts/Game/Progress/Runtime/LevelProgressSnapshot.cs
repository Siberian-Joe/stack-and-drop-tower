namespace Game.Progress.Runtime
{
    public sealed class LevelProgressSnapshot
    {
        public int CompletedLevels { get; }
        public int CurrentLevelIndex { get; }

        public LevelProgressSnapshot(int completedLevels, int currentLevelIndex)
        {
            CompletedLevels = completedLevels;
            CurrentLevelIndex = currentLevelIndex;
        }
    }
}