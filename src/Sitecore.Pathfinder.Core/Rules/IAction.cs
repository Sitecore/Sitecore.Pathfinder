// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules
{
    public interface IAction
    {
        [NotNull]
        string Name { get; }

        void Execute([NotNull] IRuleContext context, [NotNull] IDictionary<string, object> parameters);
    }
}
