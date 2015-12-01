// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Rules.Contexts
{
    public interface IProjectItemRuleContext : IRuleContext
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<IProjectItem> ProjectItems { get; }
    }
}
