// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.LogicConditions
{
    [PartNotDiscoverable]
    public class AndCondition : ConditionBase
    {
        public AndCondition([NotNull, ItemNotNull]  IEnumerable<RuleCondition> conditions) : base("and")
        {
            Conditions = conditions;
        }

        [NotNull, ItemNotNull]
        public IEnumerable<RuleCondition> Conditions { get; }

        public override bool Evaluate(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            return Conditions.All(c => c.Condition.Evaluate(ruleContext, c.Parameters));
        }
    }
}
