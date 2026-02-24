using Game.UI.BottomBar.Contracts;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class PointerAboveTopRule : ITowerPlacementRule
    {
        public int Order => 200;

        public PlacementRuleResult Evaluate(in TowerPlacementRuleContext context)
        {
            if (!context.IsManualPlacement)
                return PlacementRuleResult.Success();

            if (!context.RequirePointerBeAboveTop)
                return PlacementRuleResult.Success();

            if (context.Stack == null || context.Stack.Count == 0)
                return PlacementRuleResult.Success();

            var top = context.Stack.Top;

            return context.DesiredPivotPos.y > top.Target.y
                ? PlacementRuleResult.Success()
                : PlacementRuleResult.Fail("bottom_bar.rule.pointer_must_be_above_top");
        }
    }
}