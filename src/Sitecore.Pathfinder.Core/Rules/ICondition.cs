// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules
{
    public interface ICondition
    {
        [NotNull]
        string Name { get; }

        bool Evaluate([NotNull] IRuleContext ruleContext, [NotNull] IDictionary<string, object> parameters);
    }
}
