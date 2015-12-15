// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.ProjectTrees
{
    public interface IProjectTreeItem
    {
        [NotNull]
        string Name { get; }

        [NotNull]
        IProjectTree ProjectTree { get; }

        [NotNull]
        ProjectTreeUri Uri { get; }

        [NotNull, ItemNotNull]
        IEnumerable<IProjectTreeItem> GetChildren();
    }
}
