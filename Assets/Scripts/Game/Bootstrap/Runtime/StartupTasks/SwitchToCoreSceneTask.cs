using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scenes.Contracts;
using Game.Startup.Contracts;

namespace Game.Bootstrap.Runtime.StartupTasks
{
    public sealed class SwitchToCoreSceneTask : IAppStartupTask
    {
        public string Name => "Switch To Core Scene";
        public int Order => 1_000;

        private readonly ISceneNavigator _navigator;

        public SwitchToCoreSceneTask(ISceneNavigator navigator) => _navigator = navigator;

        public UniTask ExecuteAsync(CancellationToken token)
            => _navigator.SwitchToAsync(SceneId.Core, token);
    }
}