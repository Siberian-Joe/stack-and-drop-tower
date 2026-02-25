using DG.Tweening;
using Game.UI.BottomBar.Contracts;
using Game.UI.Screens.Contracts;
using UnityEngine;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class FailDropActionHandler : IDropActionHandler
    {
        public int Priority => 10_000;

        private readonly IGameplayWindowContext _gameplayWindow;
        private readonly BottomBarDragSession _session;

        public FailDropActionHandler(
            IGameplayWindowContext gameplayWindow,
            BottomBarDragSession session)
        {
            _gameplayWindow = gameplayWindow;
            _session = session;
        }

        public bool TryExecute()
        {
            var obj = _session.DragObject;
            var rect = _session.DragRect;

            if (obj == null || rect == null || _gameplayWindow.DragLayer == null)
                return false;

            PlayFailFallAndDestroy(obj, rect);
            return true;
        }

        private void PlayFailFallAndDestroy(GameObject obj, RectTransform rect)
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
                .OnComplete(() =>
                {
                    if (obj != null)
                        Object.Destroy(obj);
                });
        }
    }
}