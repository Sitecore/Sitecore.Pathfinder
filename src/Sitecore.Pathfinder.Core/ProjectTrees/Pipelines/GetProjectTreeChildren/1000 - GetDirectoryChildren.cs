// � 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;

namespace Sitecore.Pathfinder.ProjectTrees.Pipelines.GetProjectTreeChildren
{
    [Export(typeof(IPipelineProcessor)), Shared]
    public class GetDirectoryChildren : PipelineProcessorBase<GetProjectTreeChildrenPipeline>
    {
        public GetDirectoryChildren() : base(1000)
        {
        }

        protected override void Process(GetProjectTreeChildrenPipeline pipeline)
        {
            var directoryProjectTreeItem = pipeline.ProjectTreeItem as DirectoryProjectTreeItem;
            if (directoryProjectTreeItem == null)
            {
                return;
            }

            var projectTree = directoryProjectTreeItem.ProjectTree;
            var fileSystem = projectTree.FileSystem;

            var directory = directoryProjectTreeItem.Directory;
            if (!fileSystem.DirectoryExists(directory))
            {
                return;
            }

            foreach (var dir in fileSystem.GetDirectories(directory).OrderBy(d => d))
            {
                var directoryName = Path.GetFileName(dir);
                Assert.IsNotNullOrEmpty(directoryName);

                if (projectTree.IsDirectoryIncluded(directoryName))
                {
                    pipeline.Children.Add(new DirectoryProjectTreeItem(projectTree, dir));
                }
            }

            foreach (var fileName in fileSystem.GetFiles(directory).OrderBy(f => f))
            {
                var name = Path.GetFileName(fileName);
                Assert.IsNotNullOrEmpty(name);

                if (projectTree.IsFileIncluded(name))
                {
                    pipeline.Children.Add(new FileProjectTreeItem(projectTree, fileName));
                }
            }
        }
    }
}
