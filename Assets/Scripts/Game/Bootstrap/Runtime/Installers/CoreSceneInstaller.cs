using Cysharp.Threading.Tasks;
using Game.Lifetime.Contracts;
using Game.Lifetime.Runtime;
using Game.SceneReady.Contracts;
using Game.SceneReady.Runtime;
using Game.Startup.Contracts;
using Game.Startup.Runtime;
using Game.UI.BottomBar.Contracts;
using Game.UI.BottomBar.Runtime;
using UnityEngine;
using Zenject;

namespace Game.Bootstrap.Runtime.Installers
{
    public sealed class CoreSceneInstaller : MonoInstaller
    {
        [SerializeField] private BottomBarView _bottomBarView;
        [SerializeField] private BottomBarDragDropRefs _bottomBarDragDropRefs;

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

            Container
                .BindInstance(_bottomBarView)
                .AsSingle();

            Container.BindInstance(_bottomBarDragDropRefs).AsSingle();

            Container.Bind<BottomBarDragSession>().AsSingle();

            Container.Bind<ICubeDragInteractor>()
                .To<BottomBarDragInteractor>()
                .AsSingle();

            Container
                .Bind<ITowerStackState>()
                .To<TowerStackState>()
                .AsSingle();

            Container
                .Bind<ITowerPlacementRulesEvaluator>()
                .To<TowerPlacementRulesEvaluator>()
                .AsSingle();

            Container
                .Bind<ITowerPlacementRule>()
                .To<PointerInsideTowerDropAreaRule>()
                .AsSingle();

            Container
                .Bind<ITowerPlacementRule>()
                .To<PointerAboveTopRule>()
                .AsSingle();

            Container
                .Bind<ITowerPlacementRule>()
                .To<MaxHorizontalOffsetRule>()
                .AsSingle();

            // Container
            //     .Bind<ITowerPlacementRule>()
            //     .To<SameColorAsTopRule>()
            //     .AsSingle();

            Container
                .Bind<IDropActionHandler>()
                .To<DropIntoHoleActionHandler>()
                .AsSingle();

            Container
                .Bind<IDropActionHandler>()
                .To<PlaceIntoTowerActionHandler>()
                .AsSingle();

            Container
                .Bind<IDropActionHandler>()
                .To<FailDropActionHandler>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<InitBottomBarTask>()
                .AsSingle();
        }
    }
}