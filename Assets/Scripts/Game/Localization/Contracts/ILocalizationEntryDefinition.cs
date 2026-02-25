namespace Game.Localization.Contracts
{
    public interface ILocalizationEntryDefinition
    {
        string Key { get; }
        string Value { get; }
    }
}