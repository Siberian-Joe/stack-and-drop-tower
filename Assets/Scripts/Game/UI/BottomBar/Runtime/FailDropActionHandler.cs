using DG.Tweening;
using Game.UI.BottomBar.Contracts;
using UnityEngine;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class FailDropActionHandler : IDropActionHandler
    {
        public int Priority => 10_000;

        private readonly BottomBarDragDropRefs _refs;
        private readonly BottomBarDragSession _session;

        public FailDropActionHandler(
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

            if (obj == null || rect == null || _refs.DragLayer == null)
                return false;

            PlayFailFallAndDestroy(obj, rect);
            return true;
        }

        private void PlayFailFallAndDestroy(GameObject obj, RectTransform rect)
        {
            var cam = _refs.UiCamera;
            var screen = RectTransformUtility.WorldToScreenPoint(cam, rect.position);

            rect.SetParent(_refs.DragLayer, worldPositionStays: false);
            rect.SetAsLastSibling();

            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_refs.DragLayer, screen, cam, out var local);
            rect.anchoredPosition = local;

            var endY = _refs.DragLayer.rect.yMin - rect.rect.height - _refs.FailFallExtra;

            rect.DOKill();
            rect.DOAnchorPosY(endY, _refs.FailFallDuration)
                .SetEase(_refs.FailFallEase)
                .OnComplete(() =>
                {
                    if (obj != null)
                        Object.Destroy(obj);
                });
        }
    }
}