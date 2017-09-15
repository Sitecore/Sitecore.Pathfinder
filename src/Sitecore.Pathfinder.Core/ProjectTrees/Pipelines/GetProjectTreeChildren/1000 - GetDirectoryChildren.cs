// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.ProjectTrees.Pipelines.GetProjectTreeChildren
{
    [Export(typeof(IPipelineProcessor)), Shared]
    public class GetDirectoryChildren : PipelineProcessorBase<GetProjectTreeChildrenPipeline>
    {
        [ImportingConstructor]
        public GetDirectoryChildren([NotNull] IFactory factory) : base(1000)
        {
            Factory = factory;
        }

        [NotNull]
        protected IFactory Factory { get; }

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

            var projectDirectory = pipeline.ProjectTreeItem.ProjectTree.ProjectDirectory;

            foreach (var dir in fileSystem.GetDirectories(directory).OrderBy(d => d))
            {
                var directoryName = PathHelper.UnmapPath(projectDirectory, dir);
                Assert.IsNotNullOrEmpty(directoryName);

                if (projectTree.IsDirectoryIncluded(directoryName))
                {
                    pipeline.Children.Add(Factory.DirectoryProjectTreeItem(projectTree, dir));
                }
            }

            foreach (var fileName in fileSystem.GetFiles(directory).OrderBy(f => f))
            {
                var name = PathHelper.UnmapPath(projectDirectory, fileName);
                Assert.IsNotNullOrEmpty(name);

                if (projectTree.IsFileIncluded(name))
                {
                    pipeline.Children.Add(Factory.FileProjectTreeItem(projectTree, fileName));
                }
            }
        }
    }
}
