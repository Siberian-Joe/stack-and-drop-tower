using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.BottomBar.Runtime
{
    public static class BottomBarDragVisualUtility
    {
        private const float PosEpsilonSqr = 0.01f;

        public static void NormalizeRectForDrag(RectTransform dragRt, RectTransform sourceRt)
        {
            if (dragRt == false || sourceRt == false)
                return;

            dragRt.anchorMin = dragRt.anchorMax = new Vector2(0.5f, 0.5f);
            dragRt.pivot = sourceRt.pivot;

            dragRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sourceRt.rect.width);
            dragRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sourceRt.rect.height);

            dragRt.localRotation = Quaternion.identity;
            dragRt.localScale = Vector3.one;
        }

        public static void SetGraphicRaycasts(GameObject target, bool enabled)
        {
            if (target == false)
                return;

            foreach (var graphic in target.GetComponentsInChildren<Graphic>(true))
                graphic.raycastTarget = enabled;
        }

        public static void PlayApproachThenFall(
            RectTransform rect,
            Vector2 approachPos,
            Vector2 targetPos,
            float approachDuration,
            Ease approachEase,
            float fallDuration,
            Ease fallEase)
        {
            if (rect == false)
                return;

            rect.DOKill();

            var sequence = DOTween.Sequence()
                .SetTarget(rect)
                .SetLink(rect.gameObject);

            if (approachDuration > 0f)
            {
                var current = rect.anchoredPosition;
                if ((current - approachPos).sqrMagnitude > PosEpsilonSqr)
                    sequence.Append(rect.DOAnchorPos(approachPos, approachDuration).SetEase(approachEase));
            }
            else
            {
                rect.anchoredPosition = approachPos;
            }

            if (fallDuration > 0f)
                sequence.Append(rect.DOAnchorPos(targetPos, fallDuration).SetEase(fallEase));
            else
                rect.anchoredPosition = targetPos;
        }
    }
}