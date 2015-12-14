// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.ProjectTrees
{
    public interface IProjectTree
    {
        [CanBeNull]
        IProject Project { get; }

        [NotNull, ItemNotNull]
        IEnumerable<IProjectTreeItem> Roots { get; }
    }
}
