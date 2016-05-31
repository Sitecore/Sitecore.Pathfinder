// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects;
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

        [Diagnostics.NotNull]
        public IRuleService RuleService { get; set; }

        [NotNull]
        public ISnapshotService SnapshotService { get; set; }

        [NotNull]
        public ITraceService Trace { get; private set; }

        [Diagnostics.NotNull]
        public IXPathService XPathService { get; set; }

        [Diagnostics.NotNull]
        public Services Start([NotNull] IHostService host, [CanBeNull] Action mock = null)
        {
            Configuration = host.Configuration;
            CompositionService = host.CompositionService;

            mock?.Invoke();

            Trace = CompositionService.Resolve<ITraceService>();
            FileSystem = CompositionService.Resolve<IFileSystemService>();
            ParseService = CompositionService.Resolve<IParseService>();
            ProjectService = CompositionService.Resolve<IProjectService>();
            ConfigurationService = CompositionService.Resolve<IConfigurationService>();
            SnapshotService = CompositionService.Resolve<ISnapshotService>();
            CheckerService = CompositionService.Resolve<ICheckerService>();
            LanguageService = CompositionService.Resolve<ILanguageService>();
            RuleService = CompositionService.Resolve<IRuleService>();
            XPathService = CompositionService.Resolve<IXPathService>();

            return this;
        }
    }
}
