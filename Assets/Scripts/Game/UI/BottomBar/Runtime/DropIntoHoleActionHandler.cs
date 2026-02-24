using DG.Tweening;
using Game.UI.BottomBar.Contracts;
using UnityEngine;
using Zenject;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class DropIntoHoleActionHandler : IDropActionHandler
    {
        public int Priority => 100;

        private readonly BottomBarDragDropRefs _refs;
        private readonly BottomBarDragSession _session;

        public DropIntoHoleActionHandler(
            BottomBarDragDropRefs refs,
            BottomBarDragSession session)
        {
            _refs = refs;
            _session = session;
        }

        public bool TryExecute()
        {
            var obj = _session.DragObject;
            var rect = _session.DragRect;

            if (_refs.HoleArea == null || _refs.HoleMaskRoot == null || obj == null || rect == null)
                return false;

            if (!IsOverHoleEllipse(_session.LastScreenPoint))
                return false;

            PlayHoleFallAndDestroy(obj, rect);
            return true;
        }

        private bool IsOverHoleEllipse(Vector2 screenPoint)
        {
            var cam = _refs.UiCamera;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_refs.HoleArea, screenPoint, cam, out var local))
                return false;

            var r = _refs.HoleArea.rect;

            local -= (Vector2)r.center;

            var a = r.width * 0.5f * _refs.HoleEllipsePadding;
            var b = r.height * 0.5f * _refs.HoleEllipsePadding;

            if (a <= 0f || b <= 0f)
                return false;

            var x = local.x;
            var y = local.y;

            return (x * x) / (a * a) + (y * y) / (b * b) <= 1f;
        }

        private void PlayHoleFallAndDestroy(GameObject obj, RectTransform rect)
        {
            var cam = _refs.UiCamera;

            var cubeCenterWorld = rect.TransformPoint(rect.rect.center);
            var cubeCenterScreen = RectTransformUtility.WorldToScreenPoint(cam, cubeCenterWorld);

            rect.SetParent(_refs.HoleMaskRoot, worldPositionStays: false);
            rect.SetAsLastSibling();

            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_refs.HoleMaskRoot, cubeCenterScreen, cam, out var cubeCenterLocal);
            rect.anchoredPosition = cubeCenterLocal;

            Vector2 mouthLocal;
            if (_refs.HoleMouth != null)
            {
                var mouthCenterWorld = _refs.HoleMouth.TransformPoint(_refs.HoleMouth.rect.center);
                var mouthCenterScreen = RectTransformUtility.WorldToScreenPoint(cam, mouthCenterWorld);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_refs.HoleMaskRoot, mouthCenterScreen, cam, out mouthLocal);
            }
            else
            {
                mouthLocal = _refs.HoleMaskRoot.rect.center;
            }

            var endY = _refs.HoleMaskRoot.rect.yMin - rect.rect.height * 0.5f - _refs.HoleFallExtra;

            rect.DOKill();

            var seq = DOTween.Sequence().SetTarget(rect);
            seq.Append(rect.DOAnchorPos(mouthLocal, _refs.HolePullDuration).SetEase(_refs.HolePullEase));
            seq.Append(rect.DOAnchorPosY(endY, _refs.HoleFallDuration).SetEase(_refs.HoleFallEase));
            seq.OnComplete(() =>
            {
                if (obj != null)
                    Object.Destroy(obj);
            });
        }
    }
}