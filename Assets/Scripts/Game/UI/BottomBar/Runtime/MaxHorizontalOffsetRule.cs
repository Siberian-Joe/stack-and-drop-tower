using Game.UI.BottomBar.Contracts;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class MaxHorizontalOffsetRule : ITowerPlacementRule
    {
        public int Order => 300;

        public PlacementRuleResult Evaluate(in TowerPlacementRuleContext context)
        {
            if (context.Stack == null || context.Stack.Count == 0)
                return PlacementRuleResult.Success();

            var top = context.Stack.Top;
            var xMin = top.Target.x - top.Width * context.MaxXOffsetFactor;
            var xMax = top.Target.x + top.Width * context.MaxXOffsetFactor;
            var x = context.DesiredPivotPos.x;

            return x >= xMin && x <= xMax
                ? PlacementRuleResult.Success()
                : PlacementRuleResult.Fail("bottom_bar.rule.horizontal_offset_too_large");
        }
    }
}