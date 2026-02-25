using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Config.Contracts;
using Game.UI.BottomBar.Runtime;
using Game.UI.Screens.Contracts;
using Game.UI.Screens.Runtime.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Screens.Runtime
{
    public sealed class GameplayWindowContext : IGameplayWindowContext
    {
        private readonly IPanelService _panels;
        private readonly IGameConfigReader _config;
        private readonly IScreenRoots _screenRoots;

        private IScreenHandle<GameplayWindowView> _handle;

        public GameplayWindowContext(
            IPanelService panels,
            IGameConfigReader config,
            IScreenRoots screenRoots)
        {
            _panels = panels ?? throw new ArgumentNullException(nameof(panels));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _screenRoots = screenRoots ?? throw new ArgumentNullException(nameof(screenRoots));
        }

        public async UniTask EnsureLoadedAndOpenedAsync(CancellationToken token)
        {
            if (_handle == null || _handle.IsLoaded == false)
                _handle = await _panels.LoadAsync<GameplayWindowView>(token);

            if (_handle.IsOpen == false)
                _handle.Open();
        }

        public bool TryGetView(out GameplayWindowView view)
        {
            if (_handle != null && _handle.TryGetView(out view))
                return true;

            if (_panels.TryGetView(out view))
                return true;

            view = null;
            return false;
        }

        public GameplayWindowView View =>
            TryGetView(out var view)
                ? view
                : throw new InvalidOperationException(
                    $"{nameof(GameplayWindowView)} is not loaded. " +
                    $"Call {nameof(EnsureLoadedAndOpenedAsync)} first.");

        public BottomBarView BottomBarView => View.BottomBar;

        public ScrollRect ScrollRect => BottomBarView != null ? BottomBarView.ScrollRect : null;

        public RectTransform DragLayer => View.DragLayer;

        public RectTransform TowerRoot => View.TowerRoot;

        public RectTransform HoleArea => View.HoleArea;
        public RectTransform HoleMaskRoot => View.HoleMaskRoot;
        public RectTransform HoleMouth => View.HoleMouth;

        public Camera UiCamera =>
            _screenRoots != null && _screenRoots.Canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? _screenRoots.Canvas.worldCamera
                : null;

        public float MaxXOffsetFactor => _config.Current.CubeDrag.MaxXOffsetFactor;

        public float FallDuration => _config.Current.CubeDrag.TowerFallDuration;
        public Ease FallEase => _config.Current.CubeDrag.TowerFallEase;

        public float HoleEllipsePadding => _config.Current.CubeDrag.HoleEllipsePadding;
        public float HolePullDuration => _config.Current.CubeDrag.HolePullDuration;
        public Ease HolePullEase => _config.Current.CubeDrag.HolePullEase;

        public float HoleFallDuration => _config.Current.CubeDrag.HoleFallDuration;
        public Ease HoleFallEase => _config.Current.CubeDrag.HoleFallEase;
        public float HoleFallExtra => _config.Current.CubeDrag.HoleFallExtra;

        public float FailFallDuration => _config.Current.CubeDrag.FailFallDuration;
        public Ease FailFallEase => _config.Current.CubeDrag.FailFallEase;
        public float FailFallExtra => _config.Current.CubeDrag.FailFallExtra;
    }
}