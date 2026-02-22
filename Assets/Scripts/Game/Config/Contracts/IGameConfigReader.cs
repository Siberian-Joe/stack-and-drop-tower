namespace Game.Config.Contracts
{
    public interface IGameConfigReader
    {
        IGameConfigDefinition Current { get; }
    }
}