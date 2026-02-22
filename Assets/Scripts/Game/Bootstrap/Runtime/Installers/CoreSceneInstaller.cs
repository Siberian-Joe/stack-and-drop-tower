using Cysharp.Threading.Tasks;
using Game.Lifetime.Contracts;
using Game.Lifetime.Runtime;
using Game.SceneReady.Contracts;
using Game.SceneReady.Runtime;
using Game.Startup.Contracts;
using Game.Startup.Runtime;
using Zenject;

namespace Game.Bootstrap.Runtime.Installers
{
    public sealed class CoreSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<ISceneLifetime>()
                .FromMethod(_ => new SceneLifetime(this.GetCancellationTokenOnDestroy()))
                .AsSingle();

            Container
                .Bind<ISceneReadyGate>()
                .To<SceneReadyGate>()
                .AsSingle();

            Container
                .BindInterfacesTo<SceneReadyCoordinator>()
                .AsSingle();

            Container
                .Bind<StartupPipeline<ISceneStartupTask>>()
                .AsSingle();

            Container
                .Bind<ISceneStartup>()
                .To<SceneStartup>()
                .AsSingle();

            Container
                .BindInterfacesTo<StartupRunner<ISceneStartup, ISceneLifetime>>()
                .AsSingle();
        }
    }
}