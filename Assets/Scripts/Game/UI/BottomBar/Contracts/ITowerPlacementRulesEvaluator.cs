namespace Game.UI.BottomBar.Contracts
{
    public interface ITowerPlacementRulesEvaluator
    {
        PlacementRuleResult Evaluate(in TowerPlacementRuleContext context);
    }
}