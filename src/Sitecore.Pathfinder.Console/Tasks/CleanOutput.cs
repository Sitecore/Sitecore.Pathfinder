// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class CleanOutput : BuildTaskBase
    {
        [ImportingConstructor]
        public CleanOutput([NotNull] IConfiguration configuration, [NotNull] IFileSystem fileSystem) : base("clean-output")
        {
            Configuration = configuration;
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFileSystem FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            var outputDirectory = PathHelper.Combine(context.ProjectDirectory, Configuration.GetString(Constants.Configuration.Output.Directory));
            FileSystem.DeleteDirectory(outputDirectory);
        }
    }
}
