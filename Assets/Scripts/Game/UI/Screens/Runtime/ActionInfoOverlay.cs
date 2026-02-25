using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Localization.Contracts;
using Game.SceneReady.Contracts;
using Game.UI.Screens.Contracts;
using Game.UI.Screens.Runtime.Views;

namespace Game.UI.Screens.Runtime
{
    public sealed class ActionInfoOverlay : IActionInfoOverlay, ISceneReadyListener, IDisposable
    {
        public int Order => -9_000;

        private readonly IPanelService _panels;
        private readonly ILocalizer _localizer;

        private IScreenHandle<ActionInfoOverlayView> _handle;

        public ActionInfoOverlay(
            IPanelService panels,
            ILocalizer localizer)
        {
            _panels = panels ?? throw new ArgumentNullException(nameof(panels));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async UniTask OnSceneReadyAsync(CancellationToken token) =>
            _handle = await _panels.LoadAsync<ActionInfoOverlayView>(token);

        public void Show(string localizationKey)
        {
            if (_handle == null || _handle.IsLoaded == false)
                return;

            if (_handle.IsOpen == false)
                _handle.Open();

            var text = _localizer.TryGet(localizationKey, out var value) ? value : $"#{localizationKey}";
            _handle.View.Show(text);
        }

        public void Dispose()
        {
            _handle?.Close();
            _handle = null;
        }
    }
}