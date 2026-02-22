using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scenes.Contracts;
using Game.Scenes.Runtime;
using UnityEngine.AddressableAssets;

namespace Game.Scenes.Authoring
{
    public sealed class AddressablesSceneNavigator : ISceneNavigator
    {
        private readonly SceneCatalog _catalog;
        private readonly SemaphoreSlim _mutex = new(1, 1);

        public AddressablesSceneNavigator(SceneCatalog catalog) => _catalog = catalog;

        public async UniTask SwitchToAsync(SceneId target, CancellationToken token)
        {
            await _mutex.WaitAsync(token);
            try
            {
                await LoadSingleActivatedAsync(_catalog.Get(SceneId.Temp), token);

                var targetHandle = Addressables.LoadSceneAsync(
                    _catalog.Get(target),
                    activateOnLoad: false);

                await targetHandle.ToUniTask(cancellationToken: token);

                await targetHandle.Result.ActivateAsync();
            }
            finally
            {
                _mutex.Release();
            }
        }

        private static async UniTask LoadSingleActivatedAsync(AssetReference sceneRef, CancellationToken token)
        {
            var handle = Addressables.LoadSceneAsync(
                sceneRef,
                activateOnLoad: true);

            await handle.ToUniTask(cancellationToken: token);
        }
    }
}