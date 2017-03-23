// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitting
{
    [Export(typeof(IEmitContext))]
    public class EmitContext : IEmitContext
    {
        [ImportingConstructor]
        public EmitContext([NotNull] IConfiguration configuration, [NotNull] ITraceService traceService)
        {
            Configuration = configuration;
            Trace = traceService;
        }

        public IConfiguration Configuration { get; }

        public string ItemFormat { get; private set; } = "file";

        public IProjectBase Project { get; private set; }

        public IProjectEmitter ProjectEmitter { get; protected set; }

        public ITraceService Trace { get; }

        public virtual IEmitContext With(IProjectEmitter projectEmitter, IProjectBase project, string itemFormat)
        {
            ProjectEmitter = projectEmitter;
            Project = project;
            ItemFormat = itemFormat;

            return this;
        }
    }
}
