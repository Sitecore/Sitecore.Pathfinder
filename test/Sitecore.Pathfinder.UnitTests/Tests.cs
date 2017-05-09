// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Reflection;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder
{
    public abstract class Tests
    {
        [NotNull]
        public string ProjectDirectory { get; private set; }

        [NotNull]
        public Helpers.Services Services { get; private set; }

        protected void Start()
        {
            ProjectDirectory = Path.GetDirectoryName(GetType().GetTypeInfo().Assembly.Location);

            var app = new Startup().AsInteractive().WithToolsDirectory(ProjectDirectory).WithBinDirectory(ProjectDirectory).WithProjectDirectory(ProjectDirectory).WithConfigurationOptions(ConfigurationOptions.IncludeModuleConfig).Start();
            if (app == null)
            {
                throw new ConfigurationException(@"Oh no, nothing works!");
            }

            Services = new Helpers.Services().Start(app);
        }

    }
}
