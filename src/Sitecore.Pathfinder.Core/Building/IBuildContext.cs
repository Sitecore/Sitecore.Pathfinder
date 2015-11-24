// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Building
{
    public interface IBuildContext
    {
        bool IsBuildingWithNoConfig { get; }

        [NotNull]
        ICompositionService CompositionService { get; }

        [NotNull]
        IConfiguration Configuration { get; }

        [NotNull]
        string DataFolderDirectory { get; }

        bool DisplayDoneMessage { get; set; }

        [NotNull]
        IFileSystemService FileSystem { get; }

        bool IsAborted { get; set; }

        [NotNull]
        [ItemNotNull]
        IList<IProjectItem> ModifiedProjectItems { get; }

        [NotNull]
        [ItemNotNull]
        IList<string> OutputFiles { get; }

        [NotNull]
        IPipelineService PipelineService { get; }

        [NotNull]
        IProject Project { get; }

        [NotNull]
        string ProjectDirectory { get; }

        [CanBeNull]
        string Script { get; set; }

        [NotNull]
        string ToolsDirectory { get; }

        [NotNull]
        ITraceService Trace { get; }

        [NotNull]
        string WebsiteDirectory { get; }
    }
}
