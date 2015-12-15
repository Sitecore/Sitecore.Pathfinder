// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.ProjectTrees
{
    public interface IProjectTree
    {
        [NotNull]
        IFileSystemService FileSystem { get; }

        [NotNull, ItemNotNull]
        HashSet<string> IgnoreDirectories { get; }

        [NotNull, ItemNotNull]
        HashSet<string> IgnoreFileNames { get; }

        [NotNull]
        IPipelineService Pipelines { get; }

        [NotNull]
        string ProjectDirectory { get; }

        [NotNull, ItemNotNull]
        IEnumerable<IProjectTreeItem> Roots { get; }

        [NotNull]
        string ToolsDirectory { get; }

        [NotNull]
        IProject GetProject([NotNull] ProjectOptions projectOptions);

        [NotNull]
        IProjectTree With([NotNull] string toolsDirectory, [NotNull] string projectDirectory);
    }
}
