using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Config.Authoring;
using Game.Config.Contracts;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Config.Runtime
{
    public sealed class AddressablesGameConfigProvider : IGameConfigProvider, IDisposable
    {
        private readonly AssetReferenceT<GameConfigAsset> _reference;
        private AsyncOperationHandle<GameConfigAsset>? _handle;

        public AddressablesGameConfigProvider(AssetReferenceT<GameConfigAsset> reference) => _reference = reference;

        public async UniTask<IGameConfigDefinition> LoadAsync(CancellationToken token)
        {
            ReleaseIfNeeded();

            var handle = Addressables.LoadAssetAsync<GameConfigAsset>(_reference);
            var asset = await handle.ToUniTask(cancellationToken: token);

            _handle = handle;
            return asset;
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