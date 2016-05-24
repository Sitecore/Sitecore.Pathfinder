// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITaskContext)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TaskContext : ITaskContext
    {
        [ImportingConstructor]
        public TaskContext([NotNull] IConfiguration configuration, [NotNull] ITraceService traceService, [NotNull] IFileSystemService fileSystem)
        {
            Configuration = configuration;
            Trace = traceService;
            FileSystem = fileSystem;
        }

        public IConfiguration Configuration { get; }

        public int ErrorCode { get; set; }

        public IFileSystemService FileSystem { get; }

        public bool IsAborted { get; set; }

        public ITraceService Trace { get; }
    }
}
