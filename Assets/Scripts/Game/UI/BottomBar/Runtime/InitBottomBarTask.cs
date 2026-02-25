using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Config.Contracts;
using Game.Startup.Contracts;
using Game.UI.BottomBar.Contracts;
using Game.UI.Screens.Contracts;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class InitBottomBarTask : ISceneStartupTask
    {
        public string Name => "Init Bottom Bar";
        public int Order => 10;

        private readonly IGameConfigReader _config;
        private readonly IGameplayWindowContext _gameplayWindow;
        private readonly ICubeDragInteractor _dragInteractor;
        private readonly ICubeViewFactory _cubeFactory;

        public InitBottomBarTask(
            IGameConfigReader config,
            IGameplayWindowContext gameplayWindow,
            ICubeDragInteractor dragInteractor,
            ICubeViewFactory cubeFactory)
        {
            _config = config;
            _gameplayWindow = gameplayWindow;
            _dragInteractor = dragInteractor;
            _cubeFactory = cubeFactory;
        }

        public UniTask ExecuteAsync(CancellationToken token)
        {
            var view = _gameplayWindow.BottomBarView;
            if (view == null)
                throw new InvalidOperationException("GameplayWindow.BottomBar is not assigned");

            view.Build(_config.Current.BottomBar, _dragInteractor, _cubeFactory);
            return UniTask.CompletedTask;
        }
    }
}