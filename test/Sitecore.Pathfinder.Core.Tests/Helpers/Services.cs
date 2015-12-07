// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Querying;
using Sitecore.Pathfinder.Rules;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Xml.XPath;

namespace Sitecore.Pathfinder.Helpers
{
    public class Services
    {
        [NotNull]
        public ICheckerService CheckerService { get; set; }

        [NotNull]
        public ICompositionService CompositionService { get; private set; }

        [NotNull]
        public IConfiguration Configuration { get; private set; }

        [NotNull]
        public IConfigurationService ConfigurationService { get; private set; }

        [NotNull]
        public IFileSystemService FileSystem { get; private set; }

        [NotNull]
        public ILanguageService LanguageService { get; private set; }

        [NotNull]
        public IParseService ParseService { get; private set; }

        [NotNull]
        public IProjectService ProjectService { get; private set; }

        [NotNull]
        public IQueryService QueryService { get; set; }

        public IRuleService RuleService { get; set; }

        [NotNull]
        public ISnapshotService SnapshotService { get; set; }

        public IXPathService XPathService { get; set; }

        [NotNull]
        public ITraceService Trace { get; private set; }

        public void Start(string projectDirectory, [CanBeNull] Action mock = null)
        {
            var toolsDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

            var app = new Startup().WithToolsDirectory(toolsDirectory).WithProjectDirectory(projectDirectory).Start();
            if (app == null)
            {
                throw new ConfigurationException("Oh no, nothing works!");
            }

            Configuration = app.Configuration;
            CompositionService = app.CompositionService;

            mock?.Invoke();

            Trace = CompositionService.Resolve<ITraceService>();
            FileSystem = CompositionService.Resolve<IFileSystemService>();
            ParseService = CompositionService.Resolve<IParseService>();
            ProjectService = CompositionService.Resolve<IProjectService>();
            ConfigurationService = CompositionService.Resolve<IConfigurationService>();
            SnapshotService = CompositionService.Resolve<ISnapshotService>();
            CheckerService = CompositionService.Resolve<ICheckerService>();
            QueryService = CompositionService.Resolve<IQueryService>();
            LanguageService = CompositionService.Resolve<ILanguageService>();
            RuleService = CompositionService.Resolve<IRuleService>();
            XPathService = CompositionService.Resolve<IXPathService>();
        }
    }
}
