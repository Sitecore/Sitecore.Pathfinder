// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Rules
{
    public interface IRuleService
    {
        [NotNull, ItemNotNull]
        IEnumerable<IAction> Actions { get; }

        [NotNull, ItemNotNull]
        IEnumerable<ICondition> Conditions { get; }

        [NotNull]
        IRule ParseRule([NotNull] string configurationKey);

        [NotNull, ItemNotNull]
        IEnumerable<IRule> ParseRules([NotNull] string configurationKey);
    }
}
