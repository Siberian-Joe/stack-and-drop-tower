using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.UI.Screens.Contracts;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.UI.Screens.Runtime
{
    public sealed class AddressablesScreenPrefabProvider : IScreenPrefabProvider, IDisposable
    {
        private readonly IScreenCatalog _catalog;

        private readonly Dictionary<Type, GameObject> _prefabs = new();
        private readonly Dictionary<Type, AsyncOperationHandle<GameObject>> _handles = new();

        public AddressablesScreenPrefabProvider(IScreenCatalog catalog) =>
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));

        public async UniTask<GameObject> LoadPrefabAsync<TView>(CancellationToken token = default)
            where TView : ScreenView
        {
            var viewType = typeof(TView);

            if (_prefabs.TryGetValue(viewType, out var cached) && cached)
                return cached;

            var prefabRef = _catalog.Get<TView>();

            var handle = Addressables.LoadAssetAsync<GameObject>(prefabRef);
            var prefab = await handle.ToUniTask(cancellationToken: token);

            _handles[viewType] = handle;
            _prefabs[viewType] = prefab;

            return prefab;
        }

        public void Dispose()
        {
            foreach (var handle in _handles.Values.Where(handle => handle.IsValid()))
                Addressables.Release(handle);

            _handles.Clear();
            _prefabs.Clear();
        }
    }
}