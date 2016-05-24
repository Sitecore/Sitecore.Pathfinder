// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

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
        IList<IProjectItem> ModifiedProjectItems { get; }

        [NotNull, ItemNotNull]
        IList<string> OutputFiles { get; }

        [NotNull]
        IProject Project { get; }

        [NotNull]
        string ToolsDirectory { get; }

        [NotNull]
        string WebsiteDirectory { get; }
    }
}
