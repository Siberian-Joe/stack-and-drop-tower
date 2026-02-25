using System;
using System.Collections.Generic;
using Game.Localization.Contracts;

namespace Game.Localization.Runtime
{
    public sealed class LocalizationDefinition : ILocalizationDefinition
    {
        public string Locale { get; }
        public IReadOnlyList<ILocalizationEntryDefinition> Entries { get; }

        public LocalizationDefinition(string locale, IReadOnlyList<ILocalizationEntryDefinition> entries)
        {
            Locale = string.IsNullOrWhiteSpace(locale) ? "unknown" : locale;
            Entries = entries ?? Array.Empty<ILocalizationEntryDefinition>();
        }
    }
}