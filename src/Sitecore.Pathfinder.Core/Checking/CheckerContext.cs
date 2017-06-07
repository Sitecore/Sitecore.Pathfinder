// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.Globalization;
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
        [ImportingConstructor]
        public CheckerContext([NotNull] IConfiguration configuration, [NotNull] ITraceService trace, [NotNull] IFileSystemService fileSystem)
        {
            Trace = trace;
            FileSystem = fileSystem;

            Culture = configuration.GetCulture();
            IsDeployable = true;
        }

        public int CheckCount { get; set; }

        public IDictionary<string, CheckerSeverity> Checkers { get; } = new Dictionary<string, CheckerSeverity>();

        public CultureInfo Culture { get; }

        public IFileSystemService FileSystem { get; }

        public bool IsAborted { get; set; }

        public bool IsDeployable { get; set; }

        public IProjectBase Project { get; private set; }

        public ITraceService Trace { get; }

        public ICheckerContext With(IProjectBase project)
        {
            Project = project;

            return this;
        }
    }
}
