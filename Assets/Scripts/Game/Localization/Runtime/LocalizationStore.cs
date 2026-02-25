using System;
using System.Collections.Generic;
using Game.Localization.Contracts;

namespace Game.Localization.Runtime
{
    public sealed class LocalizationStore : ILocalizationReader, ILocalizationWriter, ILocalizer
    {
        private ILocalizationDefinition _current;
        private readonly Dictionary<string, string> _map = new(StringComparer.Ordinal);

        public ILocalizationDefinition Current =>
            _current ?? throw new InvalidOperationException("Localization is not loaded");

        public void Set(ILocalizationDefinition definition)
        {
            _current = definition ?? throw new ArgumentNullException(nameof(definition));

            _map.Clear();

            var entries = definition.Entries;
            if (entries == null)
                return;

            foreach (var entry in entries)
            {
                if (entry == null)
                    continue;

                if (string.IsNullOrWhiteSpace(entry.Key))
                    continue;

                _map[entry.Key] = entry.Value ?? string.Empty;
            }
        }

        public bool TryGet(string key, out string value)
        {
            if (string.IsNullOrWhiteSpace(key) == false)
                return _map.TryGetValue(key, out value);

            value = string.Empty;
            return false;
        }

        public string Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return string.Empty;

            return _map.TryGetValue(key, out var value)
                ? value
                : $"#{key}";
        }
    }
}