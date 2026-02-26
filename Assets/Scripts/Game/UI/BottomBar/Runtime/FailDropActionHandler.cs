using DG.Tweening;
using Game.UI.BottomBar.Contracts;
using Game.UI.Screens.Contracts;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class FailDropActionHandler : IDropActionHandler
    {
        public int Priority => 10_000;

        private readonly IGameplayWindowContext _gameplayWindow;
        private readonly IBottomBarDragSession _session;
        private readonly IActionInfoOverlay _actionInfoOverlay;
        private readonly ICubeViewFactory _cubeFactory;

        public FailDropActionHandler(
            IGameplayWindowContext gameplayWindow,
            IBottomBarDragSession session,
            IActionInfoOverlay actionInfoOverlay,
            ICubeViewFactory cubeFactory)
        {
            _gameplayWindow = gameplayWindow;
            _session = session;
            _actionInfoOverlay = actionInfoOverlay;
            _cubeFactory = cubeFactory;
        }

        public bool TryExecute()
        {
            var obj = _session.DragObject;
            var rect = _session.DragRect;

            if (obj == false || rect == false || _gameplayWindow.DragLayer == false)
                return false;

            var key = string.IsNullOrWhiteSpace(_session.LastPlacementFailureKey) == false
                ? _session.LastPlacementFailureKey
                : "bottom_bar.action.cube_disappeared";

            _actionInfoOverlay.Show(key);

            PlayFailFallAndDespawn(obj, rect);
            return true;
        }

        private void PlayFailFallAndDespawn(GameObject obj, RectTransform rect)
        {
            var cam = _gameplayWindow.UiCamera;
            var screen = RectTransformUtility.WorldToScreenPoint(cam, rect.position);

            rect.SetParent(_gameplayWindow.DragLayer, worldPositionStays: false);
            rect.SetAsLastSibling();

            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_gameplayWindow.DragLayer, screen, cam, out var local);
            rect.anchoredPosition = local;

            var endY = _gameplayWindow.DragLayer.rect.yMin - rect.rect.height - _gameplayWindow.FailFallExtra;

            rect.DOKill();
            rect.DOAnchorPosY(endY, _gameplayWindow.FailFallDuration)
                .SetEase(_gameplayWindow.FailFallEase)
                .OnComplete(() => Despawn(obj))
                .SetLink(rect.gameObject);
        }

        private void Despawn(GameObject obj)
        {
            if (obj == false)
                return;

            var view = obj.GetComponent<CubeView>();
            if (view)
            {
                _cubeFactory.Release(view);
                return;
            }

            Object.Destroy(obj);
        }
    }
}