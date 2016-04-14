// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Tasks
{
    public interface ITaskContext
    {
        [NotNull]
        ICompositionService CompositionService { get; }

        [NotNull]
        IConfiguration Configuration { get; }

        bool DisplayDoneMessage { get; set; }

        [NotNull]
        IFileSystemService FileSystem { get; }

        bool IsAborted { get; set; }

        bool IsRunningWithNoConfig { get; }

        [NotNull]
        IPipelineService PipelineService { get; }

        [NotNull]
        string Script { get; set; }

        [NotNull]
        ITraceService Trace { get; }
    }
}
