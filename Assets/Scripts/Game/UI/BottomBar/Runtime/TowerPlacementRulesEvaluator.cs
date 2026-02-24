using System.Collections.Generic;
using System.Linq;
using Game.UI.BottomBar.Contracts;

namespace Game.UI.BottomBar.Runtime
{
    public sealed class TowerPlacementRulesEvaluator : ITowerPlacementRulesEvaluator
    {
        private readonly IReadOnlyList<ITowerPlacementRule> _rules;

        public TowerPlacementRulesEvaluator(List<ITowerPlacementRule> rules)
        {
            _rules = rules
                .OrderBy(x => x.Order)
                .ToArray();
        }

        public PlacementRuleResult Evaluate(in TowerPlacementRuleContext context)
        {
            foreach (var rule in _rules)
            {
                var result = rule.Evaluate(context);
                if (!result.IsSuccess)
                    return result;
            }

            return PlacementRuleResult.Success();
        }
    }
}