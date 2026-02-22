namespace Game.Progress.Runtime
{
    public sealed class GameProgressSnapshot
    {
        public int SchemaVersion { get; }
        public TowerProgressSnapshot Tower { get; }
        public LevelProgressSnapshot Levels { get; }

        public GameProgressSnapshot(int schemaVersion, TowerProgressSnapshot tower, LevelProgressSnapshot levels)
        {
            SchemaVersion = schemaVersion;
            Tower = tower;
            Levels = levels;
        }

        public GameProgressSnapshot WithTower(TowerProgressSnapshot tower)
            => new(SchemaVersion, tower, Levels);

        public GameProgressSnapshot WithLevels(LevelProgressSnapshot levels)
            => new(SchemaVersion, Tower, levels);
    }
}