// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules
{
    [InheritedExport(typeof(ICondition))]
    public abstract class ConditionBase : ConditionActionBase, ICondition
    {
        protected ConditionBase([NotNull] string name)
        {
            Name = name;
        }

        public string Name { get; }

        public abstract bool Evaluate(IRuleContext ruleContext, IDictionary<string, object> parameters);
    }
}
