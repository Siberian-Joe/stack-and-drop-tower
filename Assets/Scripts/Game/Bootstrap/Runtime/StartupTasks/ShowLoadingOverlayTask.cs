using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Startup.Contracts;
using Game.UI.Screens.Contracts;

namespace Game.Bootstrap.Runtime.StartupTasks
{
    public sealed class ShowLoadingOverlayTask : IAppStartupTask
    {
        public string Name => "Show Loading Overlay";
        public int Order => 0;

        private readonly ISceneTransitionCurtain _transitionCurtain;

        public ShowLoadingOverlayTask(ISceneTransitionCurtain transitionCurtain) =>
            _transitionCurtain = transitionCurtain;

        public UniTask ExecuteAsync(CancellationToken token) =>
            _transitionCurtain.ShowAsync(token);
    }
}