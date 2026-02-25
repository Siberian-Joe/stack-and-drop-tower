using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Startup.Contracts;
using Game.UI.Screens.Contracts;
using Game.UI.Screens.Runtime;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Bootstrap.Runtime.StartupTasks
{
    public sealed class SpawnPersistentScreenRootsTask : IAppStartupTask
    {
        public string Name => "Spawn Persistent Screen Roots";
        public int Order => -10_000;

        private readonly IInstantiator _instantiator;
        private readonly ScreenRoots _screenRootsPrefab;
        private readonly IScreenRootsRegistry _registry;

        public SpawnPersistentScreenRootsTask(
            IInstantiator instantiator,
            ScreenRoots screenRootsPrefab,
            IScreenRootsRegistry registry)
        {
            _instantiator = instantiator;
            _screenRootsPrefab = screenRootsPrefab;
            _registry = registry;
        }

        public async UniTask ExecuteAsync(CancellationToken token)
        {
            if (_registry.IsInitialized)
                return;

            await UniTask.SwitchToMainThread(token);

            if (_screenRootsPrefab == false)
                throw new InvalidOperationException("ScreenRoots prefab is not assigned in AppInstaller");

            var instance = _instantiator.InstantiatePrefabForComponent<ScreenRoots>(_screenRootsPrefab);
            if (instance == false)
                throw new InvalidOperationException("Failed to instantiate ScreenRoots prefab");

            instance.transform.SetParent(null, worldPositionStays: false);
            Object.DontDestroyOnLoad(instance.gameObject);

            _registry.Set(instance);
        }
    }
}