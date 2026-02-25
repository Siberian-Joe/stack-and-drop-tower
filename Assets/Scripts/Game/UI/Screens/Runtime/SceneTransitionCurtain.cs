using System.Threading;
using Cysharp.Threading.Tasks;
using Game.UI.Screens.Contracts;
using Game.UI.Screens.Runtime.Views;

namespace Game.UI.Screens.Runtime
{
    public sealed class SceneTransitionCurtain : ISceneTransitionCurtain
    {
        private readonly IPanelService _panels;

        private IScreenHandle<LoadingOverlayView> _handle;

        public SceneTransitionCurtain(IPanelService panels) => _panels = panels;

        public async UniTask ShowAsync(CancellationToken token)
        {
            _handle = await _panels.LoadAsync<LoadingOverlayView>(token);
            _handle?.Open();
        }

        public void Hide() => _handle?.Close();
    }
}