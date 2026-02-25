using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Config.Contracts;
using Game.Startup.Contracts;
using Game.UI.BottomBar.Contracts;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class InitBottomBarTask : ISceneStartupTask
    {
        public string Name => "Init Bottom Bar";
        public int Order => 10;

        private readonly IGameConfigReader _config;
        private readonly BottomBarView _view;
        private readonly ICubeDragInteractor _dragInteractor;
        private readonly ICubeViewFactory _cubeFactory;

        public InitBottomBarTask(
            IGameConfigReader config,
            BottomBarView view,
            ICubeDragInteractor dragInteractor,
            ICubeViewFactory cubeFactory)
        {
            _config = config;
            _view = view;
            _dragInteractor = dragInteractor;
            _cubeFactory = cubeFactory;
        }

        public UniTask ExecuteAsync(CancellationToken token)
        {
            _view.Build(_config.Current.BottomBar, _dragInteractor, _cubeFactory);
            return UniTask.CompletedTask;
        }
    }
}