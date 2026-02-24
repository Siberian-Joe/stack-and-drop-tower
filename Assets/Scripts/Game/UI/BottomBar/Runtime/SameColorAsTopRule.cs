using System;
using Game.UI.BottomBar.Contracts;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class SameColorAsTopRule : ITowerPlacementRule
    {
        public int Order => 400;

        public PlacementRuleResult Evaluate(in TowerPlacementRuleContext context)
        {
            if (!context.IsManualPlacement)
                return PlacementRuleResult.Success();

            if (context.Stack == null || context.Stack.Count == 0)
                return PlacementRuleResult.Success();

            var top = context.Stack.Top;

            return string.Equals(top.ColorId, context.DraggedColorId, StringComparison.Ordinal)
                ? PlacementRuleResult.Success()
                : PlacementRuleResult.Fail("bottom_bar.rule.same_color_required");
        }
    }
}