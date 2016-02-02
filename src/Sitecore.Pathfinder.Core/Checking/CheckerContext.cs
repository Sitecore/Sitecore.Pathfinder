// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Globalization;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking
{
    [Export(typeof(ICheckerContext))]
    public class CheckerContext : ICheckerContext
    {
        [ImportingConstructor]
        public CheckerContext([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] IConsoleService console, [NotNull] IFactoryService factory)
        {
            Configuration = configuration;
            CompositionService = compositionService;
            Console = console;
            Factory = factory;

            Culture = configuration.GetCulture();
            IsDeployable = true;
        }

        public int CheckCount { get; set; }

        public ICompositionService CompositionService { get; }

        public int ConventionCount { get; set; }

        public CultureInfo Culture { get; }

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
