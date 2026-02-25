using Game.Localization.Contracts;

namespace Game.Localization.Runtime
{
    public sealed class LocalizationEntryDefinition : ILocalizationEntryDefinition
    {
        public string Key { get; }
        public string Value { get; }

        public LocalizationEntryDefinition(string key, string value)
        {
            Key = key ?? string.Empty;
            Value = value ?? string.Empty;
        }
    }
}