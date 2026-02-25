using System.Collections.Generic;

namespace Game.Localization.Contracts
{
    public interface ILocalizationDefinition
    {
        string Locale { get; }
        IReadOnlyList<ILocalizationEntryDefinition> Entries { get; }
    }
}