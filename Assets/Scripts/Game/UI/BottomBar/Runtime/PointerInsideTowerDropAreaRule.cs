using Game.UI.BottomBar.Contracts;
using UnityEngine;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class PointerInsideTowerDropAreaRule : ITowerPlacementRule
    {
        public int Order => 100;

        public PlacementRuleResult Evaluate(in TowerPlacementRuleContext context)
        {
            if (!context.IsManualPlacement)
                return PlacementRuleResult.Success();

            if (context.TowerDropArea == null)
                return PlacementRuleResult.Fail("bottom_bar.rule.tower_area_missing");

            var inside = RectTransformUtility.RectangleContainsScreenPoint(
                context.TowerDropArea,
                context.ScreenPoint,
                context.UiCamera);

            return inside
                ? PlacementRuleResult.Success()
                : PlacementRuleResult.Fail("bottom_bar.rule.pointer_outside_tower_area");
        }
    }
}