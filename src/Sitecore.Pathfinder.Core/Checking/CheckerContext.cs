// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Globalization;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Configuration;
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

        public CultureInfo Culture { get; }

        public IFileSystemService FileSystem { get; }

        public bool IsAborted { get; set; }

        public bool IsDeployable { get; set; }

        public IProject Project { get; private set; }

        public ITraceService Trace { get; private set; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IConsoleService Console { get; }

        [NotNull]
        protected IFactoryService Factory { get; }

        public ICheckerContext With(IProject project)
        {
            Project = project;

            Trace = new ProjectDiagnosticTraceService(Configuration, Console, Factory).With(Project);

            return this;
        }
    }
}
