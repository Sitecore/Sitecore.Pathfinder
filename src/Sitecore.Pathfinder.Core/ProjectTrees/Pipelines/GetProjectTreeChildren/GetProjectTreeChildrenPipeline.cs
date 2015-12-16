// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;

namespace Sitecore.Pathfinder.ProjectTrees.Pipelines.GetProjectTreeChildren
{
    public class GetProjectTreeChildrenPipeline : PipelineBase<GetProjectTreeChildrenPipeline>
    {
        [NotNull, ItemNotNull]
        public IList<IProjectTreeItem> Children { get; private set; }

        [NotNull]
        public IProjectTreeItem ProjectTreeItem { get; private set; }

        [NotNull]
        public GetProjectTreeChildrenPipeline Execute([NotNull] IProjectTreeItem projectTreeItem)
        {
            ProjectTreeItem = projectTreeItem;

            Children = new List<IProjectTreeItem>();

            Execute();

            return this;
        }
    }
}
