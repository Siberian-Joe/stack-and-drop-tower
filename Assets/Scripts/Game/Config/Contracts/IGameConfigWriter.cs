namespace Game.Config.Contracts
{
    public interface IGameConfigWriter
    {
        void Set(IGameConfigDefinition config);
    }
}