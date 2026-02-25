using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Startup.Contracts;
using Game.UI.Screens.Contracts;
using Game.UI.Screens.Runtime.Views;

namespace Game.Bootstrap.Runtime.StartupTasks
{
    public sealed class ShowGameplayWindowTask : ISceneStartupTask
    {
        public string Name => "Show Gameplay Window";
        public int Order => 0;

        private readonly IPanelService _panelService;

        public ShowGameplayWindowTask(IPanelService panelService) =>
            _panelService = panelService;

        public async UniTask ExecuteAsync(CancellationToken token)
        {
            var handle = await _panelService.LoadAsync<GameplayWindowView>(token);
            handle.Open();
        }
    }
}