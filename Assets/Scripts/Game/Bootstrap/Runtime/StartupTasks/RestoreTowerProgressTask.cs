using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Progress.Contracts;
using Game.Startup.Contracts;
using Game.UI.BottomBar.Contracts;
using Game.UI.BottomBar.Runtime;
using Game.UI.Screens.Contracts;
using Game.UI.Screens.Runtime;
using UnityEngine;

namespace Game.Bootstrap.Runtime.StartupTasks
{
    public sealed class RestoreTowerProgressTask : ISceneStartupTask
    {
        public string Name => "Restore Tower Progress";
        public int Order => 20;

        private readonly IProgressReader _progress;
        private readonly IGameplayWindowContext _gameplayWindow;
        private readonly ITowerStackState _towerStack;
        private readonly ICubeDragInteractor _dragInteractor;
        private readonly ICubeViewFactory _cubeFactory;

        public RestoreTowerProgressTask(
            IProgressReader progress,
            IGameplayWindowContext gameplayWindow,
            ITowerStackState towerStack,
            ICubeDragInteractor dragInteractor,
            ICubeViewFactory cubeFactory)
        {
            _progress = progress;
            _gameplayWindow = gameplayWindow;
            _towerStack = towerStack;
            _dragInteractor = dragInteractor;
            _cubeFactory = cubeFactory;
        }

        public UniTask ExecuteAsync(CancellationToken token)
        {
            var towerRoot = _gameplayWindow.TowerRoot;
            if (towerRoot == false)
                return UniTask.CompletedTask;

            var cubes = _progress.Current.Tower.Cubes;
            if (cubes == null || cubes.Count == 0)
                return UniTask.CompletedTask;

            foreach (var snapshot in cubes)
            {
                token.ThrowIfCancellationRequested();

                var cube = _cubeFactory.CreateByColorId(snapshot.ColorId, towerRoot);
                if (cube == false)
                    continue;

                cube.enabled = true;
                cube.Setup(_dragInteractor);

                var rect = (RectTransform)cube.transform;
                rect.SetParent(towerRoot, false);
                rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.localScale = Vector3.one;
                rect.localRotation = snapshot.LocalRotation;
                rect.anchoredPosition3D = snapshot.LocalPosition;

                BottomBarDragVisualUtility.SetGraphicRaycasts(cube.gameObject, true);

                _towerStack.Add(new TowerCubeState(
                    snapshot.ColorId,
                    rect,
                    rect.anchoredPosition,
                    rect.rect.width,
                    rect.rect.height));
            }

            return UniTask.CompletedTask;
        }
    }
}