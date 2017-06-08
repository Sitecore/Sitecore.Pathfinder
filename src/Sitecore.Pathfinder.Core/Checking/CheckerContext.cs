using System.Collections.Generic;
using System.Composition;
using System.Globalization;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking
{
    [Export(typeof(ICheckerContext))]
    public class CheckerContext : ICheckerContext
    {
        [FactoryConstructor]
        [ImportingConstructor]
        public CheckerContext([NotNull] IConfiguration configuration, [NotNull] IFileSystemService fileSystem)
        {
            FileSystem = fileSystem;
            Culture = configuration.GetCulture();
            IsDeployable = true;
        }

        public int CheckCount { get; set; }

        // keep this - for easy use in Checkers
        public IFileSystemService FileSystem { get; }

        public IDictionary<string, CheckerSeverity> Checkers { get; } = new Dictionary<string, CheckerSeverity>();

        public CultureInfo Culture { get; }

        public bool IsAborted { get; set; }

        public bool IsDeployable { get; set; }

        public IProjectBase Project { get; private set; } = Projects.Project.Empty;

        public ICheckerContext With(IProjectBase project)
        {
            Project = project;

            return this;
        }
    }
}
