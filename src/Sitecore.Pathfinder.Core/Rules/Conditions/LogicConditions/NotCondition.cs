// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.LogicConditions
{
    [PartNotDiscoverable]
    public class NotCondition : ConditionBase
    {
        public NotCondition([NotNull, ItemNotNull]  RuleCondition condition) : base("not")
        {
            Condition = condition;
        }

        [NotNull, ItemNotNull]
        public RuleCondition Condition { get; }

        public override bool Evaluate(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            return !Condition.Condition.Evaluate(ruleContext, Condition.Parameters);
        }
    }
}
