// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitting
{
    [Export(typeof(IEmitContext))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
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

        public IProject Project { get; private set; }

        public ITraceService Trace { get; }

        public IEmitContext With(IProject project)
        {
            Project = project;

            return this;
        }
    }
}
