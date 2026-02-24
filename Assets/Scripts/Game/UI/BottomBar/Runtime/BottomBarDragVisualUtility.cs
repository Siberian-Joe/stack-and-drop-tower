using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.BottomBar.Runtime
{
    public static class BottomBarDragVisualUtility
    {
        public static void NormalizeRectForDrag(RectTransform dragRt, RectTransform sourceRt)
        {
            if (dragRt == null || sourceRt == null)
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
            if (target == null)
                return;

            foreach (var graphic in target.GetComponentsInChildren<Graphic>(true))
                graphic.raycastTarget = enabled;
        }
    }
}