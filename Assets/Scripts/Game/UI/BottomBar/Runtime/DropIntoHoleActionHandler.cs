using DG.Tweening;
using Game.UI.BottomBar.Contracts;
using Game.UI.Screens.Contracts;
using UnityEngine;
using Zenject;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class DropIntoHoleActionHandler : IDropActionHandler
    {
        public int Priority => 100;

        private readonly IGameplayWindowContext _gameplayWindow;
        private readonly BottomBarDragSession _session;

        public DropIntoHoleActionHandler(
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

            if (_gameplayWindow.HoleArea == null || _gameplayWindow.HoleMaskRoot == null || obj == null || rect == null)
                return false;

            if (!IsOverHoleEllipse(_session.LastScreenPoint))
                return false;

            PlayHoleFallAndDestroy(obj, rect);
            return true;
        }

        private bool IsOverHoleEllipse(Vector2 screenPoint)
        {
            var cam = _gameplayWindow.UiCamera;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_gameplayWindow.HoleArea, screenPoint, cam, out var local))
                return false;

            var r = _gameplayWindow.HoleArea.rect;

            local -= (Vector2)r.center;

            var a = r.width * 0.5f * _gameplayWindow.HoleEllipsePadding;
            var b = r.height * 0.5f * _gameplayWindow.HoleEllipsePadding;

            if (a <= 0f || b <= 0f)
                return false;

            var x = local.x;
            var y = local.y;

            return (x * x) / (a * a) + (y * y) / (b * b) <= 1f;
        }

        private void PlayHoleFallAndDestroy(GameObject obj, RectTransform rect)
        {
            var cam = _gameplayWindow.UiCamera;

            var cubeCenterWorld = rect.TransformPoint(rect.rect.center);
            var cubeCenterScreen = RectTransformUtility.WorldToScreenPoint(cam, cubeCenterWorld);

            rect.SetParent(_gameplayWindow.HoleMaskRoot, worldPositionStays: false);
            rect.SetAsLastSibling();

            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_gameplayWindow.HoleMaskRoot, cubeCenterScreen, cam, out var cubeCenterLocal);
            rect.anchoredPosition = cubeCenterLocal;

            Vector2 mouthLocal;
            if (_gameplayWindow.HoleMouth != null)
            {
                var mouthCenterWorld = _gameplayWindow.HoleMouth.TransformPoint(_gameplayWindow.HoleMouth.rect.center);
                var mouthCenterScreen = RectTransformUtility.WorldToScreenPoint(cam, mouthCenterWorld);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_gameplayWindow.HoleMaskRoot, mouthCenterScreen, cam, out mouthLocal);
            }
            else
            {
                mouthLocal = _gameplayWindow.HoleMaskRoot.rect.center;
            }

            var endY = _gameplayWindow.HoleMaskRoot.rect.yMin - rect.rect.height * 0.5f - _gameplayWindow.HoleFallExtra;

            rect.DOKill();

            var seq = DOTween.Sequence().SetTarget(rect);
            seq.Append(rect.DOAnchorPos(mouthLocal, _gameplayWindow.HolePullDuration).SetEase(_gameplayWindow.HolePullEase));
            seq.Append(rect.DOAnchorPosY(endY, _gameplayWindow.HoleFallDuration).SetEase(_gameplayWindow.HoleFallEase));
            seq.OnComplete(() =>
            {
                if (obj != null)
                    Object.Destroy(obj);
            });
        }
    }
}