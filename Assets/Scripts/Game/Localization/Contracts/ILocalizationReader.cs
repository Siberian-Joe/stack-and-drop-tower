namespace Game.Localization.Contracts
{
    public interface ILocalizationReader
    {
        ILocalizationDefinition Current { get; }
    }
}