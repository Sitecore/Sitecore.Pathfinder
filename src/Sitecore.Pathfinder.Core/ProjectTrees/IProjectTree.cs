// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.ProjectTrees
{
    public interface IProjectTree
    {
        [NotNull]
        IFileSystemService FileSystem { get; }

        [NotNull]
        IPipelineService Pipelines { get; }

        [NotNull]
        string ProjectDirectory { get; }

        [NotNull, ItemNotNull]
        IEnumerable<IProjectTreeItem> Roots { get; }

        [NotNull]
        string ToolsDirectory { get; }

        [NotNull, ItemNotNull]
        IEnumerable<string> GetSourceFiles();

        bool IsDirectoryIncluded([NotNull] string directory);

        bool IsFileIncluded([NotNull] string fileName);

        [NotNull]
        IProjectTree With([NotNull] string toolsDirectory, [NotNull] string projectDirectory);
    }
}
