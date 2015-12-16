// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.ProjectTrees.Pipelines.GetProjectTreeChildren;

namespace Sitecore.Pathfinder.ProjectTrees
{
    public abstract class ProjectTreeItemBase : IProjectTreeItem
    {
        protected ProjectTreeItemBase([NotNull] IProjectTree projectTree, [NotNull] ProjectTreeUri uri)
        {
            ProjectTree = projectTree;
            Uri = uri;
        }

        public abstract string Name { get; }

        public IProjectTree ProjectTree { get; }

        public ProjectTreeUri Uri { get; }

        public IEnumerable<IProjectTreeItem> GetChildren()
        {
            return ProjectTree.Pipelines.Resolve<GetProjectTreeChildrenPipeline>().Execute(this).Children;
        }
    }
}
