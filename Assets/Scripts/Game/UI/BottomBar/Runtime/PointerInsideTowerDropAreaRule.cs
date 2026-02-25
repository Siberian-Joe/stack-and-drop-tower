using Game.UI.BottomBar.Contracts;
using UnityEngine;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class PointerInsideTowerDropAreaRule : ITowerPlacementRule
    {
        public int Order => 100;

        private readonly BottomBarDragDropRefs _refs;
        private readonly Vector3[] _corners = new Vector3[4];

        private const float ScreenPaddingPx = 2f;

        public PointerInsideTowerDropAreaRule(BottomBarDragDropRefs refs) => _refs = refs;

        public PlacementRuleResult Evaluate(in TowerPlacementRuleContext context)
        {
            if (context.IsManualPlacement == false)
                return PlacementRuleResult.Success();

            if (!TryGetCubeScreenRect(context, out var cubeScreenRect))
                return PlacementRuleResult.Fail("bottom_bar.rule.invalid_drop_area");

            if (IsCubeFullyInRightHalf(cubeScreenRect) == false)
                return PlacementRuleResult.Fail("bottom_bar.rule.pointer_must_be_in_right_half");

            return IsCubeAboveBottomPanel(cubeScreenRect, context.UiCamera) == false
                ? PlacementRuleResult.Fail("bottom_bar.rule.pointer_over_bottom_panel")
                : PlacementRuleResult.Success();
        }

        private static bool IsCubeFullyInRightHalf(Rect cubeScreenRect)
        {
            var middleX = Screen.width * 0.5f;
            return cubeScreenRect.xMin >= middleX + ScreenPaddingPx;
        }

        private bool IsCubeAboveBottomPanel(Rect cubeScreenRect, Camera uiCamera)
        {
            var panelRect = GetBottomPanelRect();
            if (panelRect == false)
                return true;

            panelRect.GetWorldCorners(_corners);

            var topLeftY = RectTransformUtility.WorldToScreenPoint(uiCamera, _corners[1]).y;
            var topRightY = RectTransformUtility.WorldToScreenPoint(uiCamera, _corners[2]).y;
            var panelTopY = Mathf.Max(topLeftY, topRightY);

            return cubeScreenRect.yMin >= panelTopY + ScreenPaddingPx;
        }

        private RectTransform GetBottomPanelRect()
        {
            if (_refs == false || _refs.ScrollRect == false)
                return null;

            if (_refs.ScrollRect.viewport)
                return _refs.ScrollRect.viewport;

            return _refs.ScrollRect.transform as RectTransform;
        }

        private static bool TryGetCubeScreenRect(in TowerPlacementRuleContext context, out Rect screenRect)
        {
            screenRect = default;

            if (context.TowerRoot == false)
                return false;

            var w = context.CubeWidth;
            var h = context.CubeHeight;

            if (w <= 0f || h <= 0f)
                return false;

            var left = context.DesiredPivotPos.x - w * context.CubePivot.x;
            var right = left + w;
            var bottom = context.DesiredPivotPos.y - h * context.CubePivot.y;
            var top = bottom + h;

            var bl = RectTransformUtility.WorldToScreenPoint(context.UiCamera,
                context.TowerRoot.TransformPoint(new Vector3(left, bottom, 0f)));
            var tl = RectTransformUtility.WorldToScreenPoint(context.UiCamera,
                context.TowerRoot.TransformPoint(new Vector3(left, top, 0f)));
            var tr = RectTransformUtility.WorldToScreenPoint(context.UiCamera,
                context.TowerRoot.TransformPoint(new Vector3(right, top, 0f)));
            var br = RectTransformUtility.WorldToScreenPoint(context.UiCamera,
                context.TowerRoot.TransformPoint(new Vector3(right, bottom, 0f)));

            var minX = Mathf.Min(bl.x, tl.x, tr.x, br.x);
            var maxX = Mathf.Max(bl.x, tl.x, tr.x, br.x);
            var minY = Mathf.Min(bl.y, tl.y, tr.y, br.y);
            var maxY = Mathf.Max(bl.y, tl.y, tr.y, br.y);

            screenRect = Rect.MinMaxRect(minX, minY, maxX, maxY);
            return true;
        }
    }
}