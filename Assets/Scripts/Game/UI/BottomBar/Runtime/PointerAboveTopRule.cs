using Game.UI.BottomBar.Contracts;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class PointerAboveTopRule : ITowerPlacementRule
    {
        public int Order => 200;

        private const float Epsilon = 0.5f;

        public PlacementRuleResult Evaluate(in TowerPlacementRuleContext context)
        {
            if (context.IsManualPlacement == false ||
                context.RequirePointerBeAboveTop == false ||
                context.Stack == null || context.Stack.Count == 0)
                return PlacementRuleResult.Success();

            var top = context.Stack.Top;

            var topPivotY = top.Rect ? top.Rect.pivot.y : 0.5f;
            var topTopY = top.Target.y + top.Height * (1f - topPivotY);

            var draggedBottomY = context.DesiredPivotPos.y - context.CubeHeight * context.CubePivot.y;

            return draggedBottomY >= topTopY - Epsilon
                ? PlacementRuleResult.Success()
                : PlacementRuleResult.Fail("bottom_bar.rule.pointer_must_be_above_top");
        }
    }
}