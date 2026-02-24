namespace Game.UI.BottomBar.Contracts
{
    public interface ITowerPlacementRule
    {
        int Order { get; }
        PlacementRuleResult Evaluate(in TowerPlacementRuleContext context);
    }
}