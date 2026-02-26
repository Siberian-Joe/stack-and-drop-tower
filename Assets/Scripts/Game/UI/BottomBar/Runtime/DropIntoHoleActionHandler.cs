using DG.Tweening;
using Game.UI.BottomBar.Contracts;
using Game.UI.Screens.Contracts;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class DropIntoHoleActionHandler : IDropActionHandler
    {
        public int Priority => 100;

        private readonly IGameplayWindowContext _gameplayWindow;
        private readonly IBottomBarDragSession _session;
        private readonly IActionInfoOverlay _actionInfoOverlay;
        private readonly ICubeViewFactory _cubeFactory;

        public DropIntoHoleActionHandler(
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

            if (_gameplayWindow.HoleArea == false || _gameplayWindow.HoleMaskRoot == false || obj == false || rect == false)
                return false;

            if (IsOverHoleEllipse(_session.LastScreenPoint) == false)
                return false;

            PlayHoleFallAndDespawn(obj, rect);
            _actionInfoOverlay.Show("bottom_bar.action.dropped_into_hole");

            return true;
        }

        private bool IsOverHoleEllipse(Vector2 screenPoint)
        {
            var cam = _gameplayWindow.UiCamera;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_gameplayWindow.HoleArea, screenPoint, cam, out var local) == false)
                return false;

            var rect = _gameplayWindow.HoleArea.rect;
            local -= rect.center;

            var a = rect.width * 0.5f * _gameplayWindow.HoleEllipsePadding;
            var b = rect.height * 0.5f * _gameplayWindow.HoleEllipsePadding;

            if (a <= 0f || b <= 0f)
                return false;

            var x = local.x;
            var y = local.y;

            return x * x / (a * a) + y * y / (b * b) <= 1f;
        }

        private void PlayHoleFallAndDespawn(GameObject obj, RectTransform rect)
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
            if (_gameplayWindow.HoleMouth)
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

            DOTween.Sequence()
                .SetTarget(rect)
                .Append(rect
                    .DOAnchorPos(mouthLocal, _gameplayWindow.HolePullDuration)
                    .SetEase(_gameplayWindow.HolePullEase))
                .Append(rect
                    .DOAnchorPosY(endY, _gameplayWindow.HoleFallDuration)
                    .SetEase(_gameplayWindow.HoleFallEase))
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