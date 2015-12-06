// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules
{
    public class Rule : IRule
    {
        public Rule([NotNull] string filter)
        {
            Filter = filter;
        }

        [NotNull, ItemNotNull]
        public ICollection<RuleAction> Else { get; } = new List<RuleAction>();

        public string Filter { get; }

        [NotNull, ItemNotNull]
        public ICollection<RuleCondition> If { get; } = new List<RuleCondition>();

        [NotNull, ItemNotNull]
        public ICollection<RuleAction> Then { get; } = new List<RuleAction>();

        public bool EvaluateIf(IRuleContext context)
        {
            return If.All(clause => clause.Condition.Evaluate(context, clause.Parameters));
        }

        public bool Execute(IRuleContext context)
        {
            if (EvaluateIf(context))
            {
                ExecuteThen(context);
                return true;
            }

            ExecuteElse(context);
            return false;
        }

        public void ExecuteElse(IRuleContext context)
        {
            foreach (var clause in Else)
            {
                clause.Action.Execute(context, clause.Parameters);
            }
        }

        public void ExecuteThen(IRuleContext context)
        {
            foreach (var clause in Then)
            {
                clause.Action.Execute(context, clause.Parameters);
            }
        }
    }
}
