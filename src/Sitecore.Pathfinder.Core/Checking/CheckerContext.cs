// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking
{
    [Export(typeof(ICheckerContext)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CheckerContext : ICheckerContext
    {
        [ImportingConstructor]
        public CheckerContext([NotNull] IConfiguration configuration, [NotNull] IConsoleService console, [NotNull] IFileSystemService fileSystem, [NotNull] IFactoryService factory)
        {
            Configuration = configuration;
            Console = console;
            FileSystem = fileSystem;
            Factory = factory;

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

        public ITraceService Trace { get; private set; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IConsoleService Console { get; }

        [NotNull]
        protected IFactoryService Factory { get; }

        public ICheckerContext With(IProjectBase project, IDiagnosticCollector collector)
        {
            Project = project;

            Trace = new DiagnosticTraceService(Configuration, Console, Factory).With(collector);

            return this;
        }
    }
}
