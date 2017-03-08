// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Tasks.Building
{
    public interface IBuildContext : ITaskContext
    {
        [NotNull]
        string DataFolderDirectory { get; }

        bool IsProjectLoaded { get; }

        [NotNull, ItemNotNull]
        ICollection<IProjectItem> ModifiedProjectItems { get; }

        [NotNull, ItemNotNull]
        ICollection<OutputFile> OutputFiles { get; }

        [NotNull]
        string ProjectDirectory { get; }

        [NotNull]
        string ToolsDirectory { get; }

        [NotNull]
        IProject LoadProject();

        [NotNull]
        IBuildContext With([NotNull] Func<IProject> loadProject);
    }
}
