using Cysharp.Threading.Tasks;
using Game.Bootstrap.Runtime.StartupTasks;
using Game.Config.Authoring;
using Game.Config.Contracts;
using Game.Config.Runtime;
using Game.Lifetime.Contracts;
using Game.Lifetime.Runtime;
using Game.Localization.Contracts;
using Game.Localization.Runtime;
using Game.Progress.Contracts;
using Game.Progress.Runtime;
using Game.SceneReady.Contracts;
using Game.SceneReady.Runtime;
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

        [Header("Localization")] [SerializeField]
        private AssetReferenceT<TextAsset> _localizationJson;

        public override void InstallBindings()
        {
            InstallLifetime();

            InstallConfig();
            InstallScenes();
            InstallProgress();

            InstallScreens();
            InstallLocalization();

            InstallStartup();
        }

        private void InstallLifetime()
        {
            Container
                .Bind<IAppLifetime>()
                .FromMethod(_ => new AppLifetime(this.GetCancellationTokenOnDestroy()))
                .AsSingle();
        }

        private void InstallConfig()
        {
            Container
                .BindInstance(_gameConfig)
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
        }

        private void InstallScenes()
        {
            Container
                .BindInstance(_sceneCatalog)
                .AsSingle();

            Container
                .Bind<ISceneNavigator>()
                .To<AddressablesSceneNavigator>()
                .AsSingle();

            Container
                .Bind<ISceneTransitionCurtain>()
                .To<SceneTransitionCurtain>()
                .AsSingle();
        }

        private void InstallProgress()
        {
            Container
                .BindInterfacesTo<ProgressStore>()
                .AsSingle();

            Container
                .Bind<IProgressRepository>()
                .To<JsonFileProgressRepository>()
                .AsSingle();
        }

        private void InstallScreens()
        {
            Container
                .Bind<ScreenRoots>()
                .FromInstance(_screenRootsPrefab)
                .WhenInjectedInto<SpawnPersistentScreenRootsTask>();

            Container
                .BindInterfacesAndSelfTo<ScreenRootsRegistry>()
                .AsSingle();

            Container
                .Bind<IScreenCatalog>()
                .FromInstance(_screenCatalog)
                .AsSingle();

            Container
                .BindInterfacesTo<AddressablesScreenPrefabProvider>()
                .AsSingle();

            Container
                .BindInterfacesTo<PanelService>()
                .AsSingle();
        }

        private void InstallLocalization()
        {
            Container
                .BindInstance(_localizationJson)
                .AsSingle();

            Container
                .Bind<ILocalizationProvider>()
                .To<AddressablesJsonLocalizationProvider>()
                .AsSingle();

            Container
                .BindInterfacesTo<LocalizationStore>()
                .AsSingle();

            Container
                .BindInterfacesTo<LoadLocalizationTask>()
                .AsSingle();
        }

        private void InstallStartup()
        {
            Container
                .Bind<IAppReadyGate>()
                .To<AppReadyGate>()
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
                .BindInterfacesTo<ShowLoadingOverlayTask>()
                .AsSingle();

            Container
                .BindInterfacesTo<SpawnPersistentScreenRootsTask>()
                .AsSingle();

            Container
                .BindInterfacesTo<LoadGameConfigTask>()
                .AsSingle();

            Container
                .BindInterfacesTo<LoadProgressTask>()
                .AsSingle();

            Container
                .BindInterfacesTo<OpenAppReadyGateTask>()
                .AsSingle();

            Container
                .BindInterfacesTo<SwitchToCoreSceneTask>()
                .AsSingle();
        }
    }
}