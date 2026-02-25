using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Localization.Contracts;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Localization.Runtime
{
    public sealed class AddressablesJsonLocalizationProvider : ILocalizationProvider, IDisposable
    {
        [Serializable]
        private sealed class LocalizationJsonDto
        {
            public string Locale;
            public LocalizationEntryDto[] Entries;
        }

        [Serializable]
        private sealed class LocalizationEntryDto
        {
            public string Key;
            public string Value;
        }

        private readonly AssetReferenceT<TextAsset> _reference;
        private AsyncOperationHandle<TextAsset>? _handle;

        public AddressablesJsonLocalizationProvider(AssetReferenceT<TextAsset> reference) => _reference = reference;

        public async UniTask<ILocalizationDefinition> LoadAsync(CancellationToken token)
        {
            ReleaseIfNeeded();

            var handle = Addressables.LoadAssetAsync<TextAsset>(_reference);
            var asset = await handle.ToUniTask(cancellationToken: token);

            _handle = handle;

            if (asset == false)
                throw new InvalidOperationException("Localization TextAsset is null");

            if (string.IsNullOrWhiteSpace(asset.text))
                throw new InvalidOperationException("Localization JSON is empty");

            var dto = JsonUtility.FromJson<LocalizationJsonDto>(asset.text);
            if (dto == null)
                throw new InvalidOperationException("Failed to parse localization JSON");

            var entries = new List<ILocalizationEntryDefinition>();

            if (dto.Entries != null)
            {
                entries.AddRange(
                    dto.Entries
                        .Where(x => x != null && string.IsNullOrWhiteSpace(x.Key) == false)
                        .Select(x => (ILocalizationEntryDefinition)new LocalizationEntryDefinition(x.Key, x.Value)));
            }

            return new LocalizationDefinition(dto.Locale, entries);
        }

        public void Dispose() => ReleaseIfNeeded();

        private void ReleaseIfNeeded()
        {
            if (_handle.HasValue && _handle.Value.IsValid())
                Addressables.Release(_handle.Value);

            _handle = null;
        }
    }
}