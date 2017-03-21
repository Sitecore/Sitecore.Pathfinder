// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITaskContext))]
    public class TaskContext : ITaskContext
    {
        [ImportingConstructor]
        public TaskContext([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] IConsoleService console, [NotNull] ITraceService traceService, [NotNull] IFileSystemService fileSystem)
        {
            Configuration = configuration;
            CompositionService = compositionService;
            Console = console;
            Trace = traceService;
            FileSystem = fileSystem;
        }

        public ICompositionService CompositionService { get; }

        public IConfiguration Configuration { get; }

        public IConsoleService Console { get; }

        public int ErrorCode { get; set; }

        public IFileSystemService FileSystem { get; }

        public bool IsAborted { get; set; }

        public ITraceService Trace { get; }
    }
}
