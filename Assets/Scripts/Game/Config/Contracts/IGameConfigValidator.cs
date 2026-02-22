namespace Game.Config.Contracts
{
    public interface IGameConfigValidator
    {
        void Validate(IGameConfigDefinition config);
    }
}