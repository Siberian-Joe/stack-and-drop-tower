using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Startup.Contracts;
using Game.UI.Screens.Contracts;

namespace Game.Bootstrap.Runtime.StartupTasks
{
    public sealed class ShowGameplayWindowTask : ISceneStartupTask
    {
        public string Name => "Show Gameplay Window";
        public int Order => 0;

        private readonly IGameplayWindowContext _gameplayWindow;

        public ShowGameplayWindowTask(IGameplayWindowContext gameplayWindow) =>
            _gameplayWindow = gameplayWindow;

        public UniTask ExecuteAsync(CancellationToken token) =>
            _gameplayWindow.EnsureLoadedAndOpenedAsync(token);
    }
}