using Cysharp.Threading.Tasks;
using Game.Bootstrap.Runtime.SceneReady;
using Game.Bootstrap.Runtime.StartupTasks;
using Game.Lifetime.Contracts;
using Game.Lifetime.Runtime;
using Game.Progress.Runtime.Listeners;
using Game.SceneReady.Contracts;
using Game.SceneReady.Runtime;
using Game.Startup.Contracts;
using Game.Startup.Runtime;
using Game.UI.BottomBar.Contracts;
using Game.UI.BottomBar.Runtime;
using Game.UI.Screens.Runtime;
using Zenject;

namespace Game.Bootstrap.Runtime.Installers
{
    public sealed class CoreSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InstallLifetime();

            InstallSceneReady();
            InstallSceneStartup();

            InstallGameplayUiContext();
            InstallBottomBarDragAndDrop();

            InstallTowerPlacementRules();
            InstallDropActionHandlers();

            InstallProgressSync();
            InstallSceneReadyListeners();
            InstallSceneStartupTasks();
        }

        private void InstallLifetime()
        {
            Container
                .Bind<ISceneLifetime>()
                .FromMethod(_ => new SceneLifetime(this.GetCancellationTokenOnDestroy()))
                .AsSingle();
        }

        private void InstallSceneReady()
        {
            Container
                .Bind<ISceneReadyGate>()
                .To<SceneReadyGate>()
                .AsSingle();

            Container
                .BindInterfacesTo<SceneReadyCoordinator>()
                .AsSingle();
        }

        private void InstallSceneStartup()
        {
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

        private void InstallGameplayUiContext()
        {
            Container
                .BindInterfacesTo<GameplayWindowContext>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<ActionInfoOverlay>()
                .AsSingle();
        }

        private void InstallBottomBarDragAndDrop()
        {
            Container
                .BindInterfacesAndSelfTo<BottomBarDragSession>()
                .AsSingle();

            Container
                .Bind<ICubeViewFactory>()
                .To<CubeViewFactory>()
                .AsSingle();

            Container
                .Bind<ICubeDragInteractor>()
                .To<BottomBarDragInteractor>()
                .AsSingle();

            Container
                .BindInterfacesTo<TowerStackState>()
                .AsSingle();
        }

        private void InstallTowerPlacementRules()
        {
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
        }

        private void InstallDropActionHandlers()
        {
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
        }

        private void InstallProgressSync()
        {
            Container
                .BindInterfacesTo<TowerProgressToStoreBridge>()
                .AsSingle();

            Container
                .BindInterfacesTo<AutoSaveListener>()
                .AsSingle();
        }

        private void InstallSceneReadyListeners()
        {
            Container
                .Bind<ISceneReadyListener>()
                .To<CloseLoadingOverlayOnSceneReadyListener>()
                .AsSingle();
        }

        private void InstallSceneStartupTasks()
        {
            Container
                .BindInterfacesTo<ShowGameplayWindowTask>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<InitBottomBarTask>()
                .AsSingle();

            Container
                .Bind<ISceneStartupTask>()
                .To<RestoreTowerProgressTask>()
                .AsSingle();
        }
    }
}