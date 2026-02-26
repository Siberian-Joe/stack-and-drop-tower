using System;
using Game.UI.Screens.Contracts;

namespace Game.UI.Screens.Runtime
{
    public sealed class ScreenHandle<TView> : IScreenHandle<TView> where TView : ScreenView
    {
        private readonly PanelService _service;

        public ScreenHandle(PanelService service) =>
            _service = service ?? throw new ArgumentNullException(nameof(service));

        public Type ViewType => typeof(TView);
        public bool IsLoaded => _service.IsLoaded(typeof(TView));
        public bool IsOpen => _service.IsOpen(typeof(TView));

        public TView View =>
            _service.TryGetView(out TView view) == false
                ? throw new InvalidOperationException($"Screen is not loaded: {typeof(TView).FullName}")
                : view;

        public bool TryGetView(out TView view) => _service.TryGetView(out view);

        public void Open() => _service.Open<TView>();
        public void Close() => _service.Close<TView>();
    }
}