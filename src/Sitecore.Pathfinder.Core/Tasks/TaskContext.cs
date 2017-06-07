// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITaskContext))]
    public class TaskContext : ITaskContext
    {
        [ImportingConstructor]
        public TaskContext([NotNull] IConfiguration configuration, [NotNull] IConsoleService console, [NotNull] ITraceService traceService)
        {
            Configuration = configuration;
            Console = console;
            Trace = traceService;
        }

        // keep this - for easy use in Tasks
        public IConfiguration Configuration { get; }

        // keep this - for easy use in Tasks
        public IConsoleService Console { get; }

        public int ErrorCode { get; set; }

        public bool IsAborted { get; set; }

        // keep this - for easy use in Tasks
        public ITraceService Trace { get; }
    }
}
