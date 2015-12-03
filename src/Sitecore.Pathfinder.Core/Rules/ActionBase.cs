// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules
{
    [InheritedExport(typeof(IAction))]
    public abstract class ActionBase : ConditionActionBase, IAction
    {
        protected ActionBase([NotNull] string name)
        {
            Name = name;
        }

        public string Name { get; }

        public abstract void Execute(IRuleContext context, IDictionary<string, object> parameters);
    }
}
