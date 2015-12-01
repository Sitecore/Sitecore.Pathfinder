// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Rules
{
    public class RuleCondition
    {
        public RuleCondition([NotNull] ICondition condition, [NotNull] IDictionary<string, string> parameters)
        {
            Condition = condition;
            Parameters = parameters;
        }

        [NotNull]
        public ICondition Condition { get; }

        [NotNull]
        public IDictionary<string, string> Parameters { get; }
    }
}
