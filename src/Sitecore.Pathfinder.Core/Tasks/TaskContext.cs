// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITaskContext)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TaskContext : ITaskContext
    {
        [ImportingConstructor]
        public TaskContext([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] ITraceService traceService, [NotNull] IFileSystemService fileSystem, [NotNull] IPipelineService pipelineService)
        {
            CompositionService = compositionService;
            Configuration = configuration;
            Trace = traceService;
            FileSystem = fileSystem;
            PipelineService = pipelineService;
        }

        public ICompositionService CompositionService { get; }

        public IConfiguration Configuration { get; }

        public bool DisplayDoneMessage { get; set; } = true;

        public IFileSystemService FileSystem { get; }

        public bool IsAborted { get; set; }

        public bool IsRunningWithNoConfig => Configuration.GetBool(Constants.Configuration.BuildingWithNoConfig);

        public IPipelineService PipelineService { get; }

        public string Script { get; set; }

        public ITraceService Trace { get; }
    }
}
