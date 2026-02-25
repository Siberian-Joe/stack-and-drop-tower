using System;
using Cysharp.Threading.Tasks;
using Game.Bootstrap.Runtime.StartupTasks;
using Game.Config.Authoring;
using Game.Config.Contracts;
using Game.Config.Runtime;
using Game.Lifetime.Contracts;
using Game.Lifetime.Runtime;
using Game.Progress.Contracts;
using Game.Progress.Runtime;
using Game.Scenes.Authoring;
using Game.Scenes.Contracts;
using Game.Scenes.Runtime;
using Game.Startup.Contracts;
using Game.Startup.Runtime;
using Game.UI.Screens.Contracts;
using Game.UI.Screens.Runtime;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Game.Bootstrap.Runtime.Installers
{
    public sealed class AppInstaller : MonoInstaller
    {
        [Header("Config")] [SerializeField] private AssetReferenceT<GameConfigAsset> _gameConfig;

        [Header("Scenes")] [SerializeField] private SceneCatalog _sceneCatalog;

        [Header("Screens")] [SerializeField] private ScreenRoots _screenRootsPrefab;
        [SerializeField] private ScreenCatalog _screenCatalog;

        public override void InstallBindings()
        {
            Container
                .Bind<IAppLifetime>()
                .FromMethod(_ => new AppLifetime(this.GetCancellationTokenOnDestroy()))
                .AsSingle();

            Container
                .BindInstance(_gameConfig)
                .AsSingle();

            Container
                .BindInstance(_sceneCatalog)
                .AsSingle();

            Container
                .BindInterfacesTo<GameConfigStore>()
                .AsSingle();

            Container
                .Bind<IGameConfigProvider>()
                .To<AddressablesGameConfigProvider>()
                .AsSingle();

            Container
                .Bind<IGameConfigValidator>()
                .To<GameConfigValidator>()
                .AsSingle();

            Container
                .Bind<ISceneTransitionCurtain>()
                .To<SceneTransitionCurtain>()
                .AsSingle();

            Container
                .Bind<ISceneNavigator>()
                .To<AddressablesSceneNavigator>()
                .AsSingle();

            Container
                .BindInterfacesTo<ProgressStore>()
                .AsSingle();

            Container
                .Bind<IProgressRepository>()
                .To<JsonFileProgressRepository>()
                .AsSingle();

            Container
                .Bind<StartupPipeline<IAppStartupTask>>()
                .AsSingle();

            Container
                .Bind<IAppStartup>()
                .To<AppStartup>()
                .AsSingle();

            Container
                .BindInterfacesTo<StartupRunner<IAppStartup, IAppLifetime>>()
                .AsSingle();

            Container
                .BindInterfacesTo<LoadGameConfigTask>()
                .AsSingle();

            Container
                .BindInterfacesTo<LoadProgressTask>()
                .AsSingle();

            Container
                .BindInterfacesTo<SwitchToCoreSceneTask>()
                .AsSingle();

            Container
                .BindInterfacesTo<ShowLoadingOverlayTask>()
                .AsSingle();

            Container
                .Bind<ScreenRoots>()
                .FromInstance(_screenRootsPrefab)
                .WhenInjectedInto<SpawnPersistentScreenRootsTask>();

            Container
                .BindInterfacesAndSelfTo<ScreenRootsRegistry>()
                .AsSingle();

            Container
                .BindInterfacesTo<SpawnPersistentScreenRootsTask>()
                .AsSingle();

            Container
                .Bind<IScreenCatalog>()
                .FromInstance(_screenCatalog)
                .AsSingle();

            Container
                .BindInterfacesTo<AddressablesPanelService>()
                .AsSingle();
        }
    }
}