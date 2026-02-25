using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Progress.Contracts;
using Game.Startup.Contracts;
using Game.UI.BottomBar.Contracts;
using Game.UI.BottomBar.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Bootstrap.Runtime.StartupTasks
{
    public sealed class RestoreTowerProgressTask : ISceneStartupTask
    {
        public string Name => "Restore Tower Progress";
        public int Order => 20;

        private readonly IProgressReader _progress;
        private readonly BottomBarDragDropRefs _refs;
        private readonly ITowerStackState _towerStack;
        private readonly ICubeDragInteractor _dragInteractor;
        private readonly ICubeViewFactory _cubeFactory;

        public RestoreTowerProgressTask(
            IProgressReader progress,
            BottomBarDragDropRefs refs,
            ITowerStackState towerStack,
            ICubeDragInteractor dragInteractor,
            ICubeViewFactory cubeFactory)
        {
            _progress = progress;
            _refs = refs;
            _towerStack = towerStack;
            _dragInteractor = dragInteractor;
            _cubeFactory = cubeFactory;
        }

        public UniTask ExecuteAsync(CancellationToken token)
        {
            if (_refs.TowerRoot == false)
                return UniTask.CompletedTask;

            var cubes = _progress.Current.Tower.Cubes;
            if (cubes == null || cubes.Count == 0)
                return UniTask.CompletedTask;

            for (var i = 0; i < cubes.Count; i++)
            {
                token.ThrowIfCancellationRequested();

                var snapshot = cubes[i];
                var cube = _cubeFactory.CreateByColorId(snapshot.ColorId, _refs.TowerRoot);
                if (cube == null)
                    continue;

                cube.enabled = true;
                cube.Setup(_dragInteractor);

                var rect = (RectTransform)cube.transform;
                rect.SetParent(_refs.TowerRoot, false);
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