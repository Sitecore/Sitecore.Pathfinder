// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Rules
{
    public class RuleAction
    {
        public RuleAction([NotNull] IAction action, [NotNull] IDictionary<string, object> parameters)
        {
            Action = action;
            Parameters = parameters;
        }

        [NotNull]
        public IAction Action { get; }

        [NotNull]
        public IDictionary<string, object> Parameters { get; }
    }
}
