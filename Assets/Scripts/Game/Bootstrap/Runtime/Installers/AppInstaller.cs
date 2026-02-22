using Cysharp.Threading.Tasks;
using Game.Bootstrap.Runtime.StartupTasks;
using Game.Config.Authoring;
using Game.Config.Contracts;
using Game.Config.Runtime;
using Game.Lifetime.Contracts;
using Game.Lifetime.Runtime;
using Game.Scenes.Authoring;
using Game.Scenes.Contracts;
using Game.Scenes.Runtime;
using Game.Startup.Contracts;
using Game.Startup.Runtime;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Game.Bootstrap.Runtime.Installers
{
    public sealed class AppInstaller : MonoInstaller
    {
        [Header("Config")] [SerializeField] private AssetReferenceT<GameConfigAsset> _gameConfig;

        [Header("Scenes")] [SerializeField] private SceneCatalog _sceneCatalog;

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
                .Bind<ISceneNavigator>()
                .To<AddressablesSceneNavigator>()
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
                .Bind<IAppStartupTask>()
                .To<LoadGameConfigTask>()
                .AsSingle();

            Container
                .Bind<IAppStartupTask>()
                .To<SwitchToCoreSceneTask>()
                .AsSingle();
        }
    }
}