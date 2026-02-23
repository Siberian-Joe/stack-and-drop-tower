using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Config.Contracts;
using Game.Startup.Contracts;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class InitBottomBarTask : ISceneStartupTask
    {
        public string Name => "Init Bottom Bar";
        public int Order => 10;

        private readonly IGameConfigReader _config;
        private readonly BottomBarView _view;

        public InitBottomBarTask(IGameConfigReader config, BottomBarView view)
        {
            _config = config;
            _view = view;
        }

        public UniTask ExecuteAsync(CancellationToken token)
        {
            var config = _config.Current;
            _view.Build(config.BottomBar);

            return UniTask.CompletedTask;
        }
    }
}