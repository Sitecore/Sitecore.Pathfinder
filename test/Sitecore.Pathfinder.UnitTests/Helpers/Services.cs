// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Helpers
{
    public class Services
    {
        [NotNull]
        public ICheckerService CheckerService { get; set; }

        [NotNull]
        public IConfiguration Configuration { get; private set; }

        [NotNull]
        public IConfigurationService ConfigurationService { get; private set; }

        [NotNull]
        public IFileSystemService FileSystem { get; private set; }

        [NotNull]
        public IFactory Factory { get; private set; }

        [NotNull]
        public IParseService ParseService { get; private set; }

        [NotNull]
        public IProjectService ProjectService { get; private set; }

        [NotNull]
        public ISnapshotService SnapshotService { get; set; }

        [NotNull]
        public ITraceService Trace { get; private set; }

        [Diagnostics.NotNull]
        public Services Start([NotNull] IHostService host)
        {
            Configuration = host.Configuration;
            Factory = host.Factory;
            Trace = Factory.Resolve<ITraceService>();
            FileSystem = Factory.Resolve<IFileSystemService>();
            ParseService = Factory.Resolve<IParseService>();
            ProjectService = Factory.Resolve<IProjectService>();
            ConfigurationService = Factory.Resolve<IConfigurationService>();
            SnapshotService = Factory.Resolve<ISnapshotService>();
            CheckerService = Factory.Resolve<ICheckerService>();

            return this;
        }
    }
}
