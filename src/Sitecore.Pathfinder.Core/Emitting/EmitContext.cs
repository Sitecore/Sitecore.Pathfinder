// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitting
{
    [Export(typeof(IEmitContext)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class EmitContext : IEmitContext
    {
        [ImportingConstructor]
        public EmitContext([NotNull] IConfiguration configuration, [NotNull] ITraceService traceService, [NotNull] IFileSystemService fileSystemService)
        {
            Configuration = configuration;
            Trace = traceService;
            FileSystem = fileSystemService;

            ForceUpdate = Configuration.GetBool(Constants.Configuration.BuildProject.ForceUpdate, true);
        }

        public IConfiguration Configuration { get; }

        public IFileSystemService FileSystem { get; }

        public bool ForceUpdate { get; }

        public IProjectBase Project { get; private set; }

        public IProjectEmitter ProjectEmitter { get; protected set; }

        public ITraceService Trace { get; }

        public virtual IEmitContext With(IProjectEmitter projectEmitter, IProjectBase project)
        {
            ProjectEmitter = projectEmitter;
            Project = project;

            return this;
        }
    }
}
