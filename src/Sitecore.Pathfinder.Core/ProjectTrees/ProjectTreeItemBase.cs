// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.ProjectTrees
{
    public abstract class ProjectTreeItemBase : IProjectTreeItem
    {                           
        protected ProjectTreeItemBase([NotNull] ProjectTreeUri uri)
        {
            Uri = uri;
        }

        public abstract string Name { get; }

        public abstract IProjectTreeItem ParentItem { get; }

        public ProjectTreeUri Uri { get; }

        public abstract IEnumerable<IProjectTreeItem> GetChildren();
    }
}
