// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder
{
    public abstract class Tests
    {
        public const string BadWebsite = "Website.Bad";

        public const string GoodWebsite = "Website.Good";

        [Diagnostics.NotNull]
        public string ProjectDirectory { get; private set; } = string.Empty;

        [Diagnostics.NotNull]
        public Helpers.Services Services { get; private set; }

        protected void Mock<T>([Diagnostics.NotNull] T value)
        {
            Services.CompositionService.Set(value);
        }

        [Diagnostics.NotNull]
        protected T Resolve<T>()
        {
            return Services.CompositionService.Resolve<T>();
        }

        protected void Start([Diagnostics.NotNull] string website, [Diagnostics.CanBeNull] Action mock = null)
        {
            var toolsDirectory = Directory.GetCurrentDirectory();

            // 3 levels up
            var rootDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
            ProjectDirectory = PathHelper.Combine(rootDirectory ?? string.Empty, website);

            // add scc.exe for tasks
            var assemblies = new List<string>()
            {
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "scc.exe")
            };

            var app = new Startup().AsInteractive().WithToolsDirectory(toolsDirectory).WithProjectDirectory(ProjectDirectory).WithConfigurationOptions(ConfigurationOptions.IncludeModuleConfig).WithAssemblies(assemblies).Start();
            if (app == null)
            {
                throw new ConfigurationException(@"Oh no, nothing works!");
            }

            Services = new Helpers.Services().Start(app, mock);

            var restorePackages = app.CompositionService.ResolveMany<ITask>().First(t => t is RestorePackages);
            var context = app.CompositionService.Resolve<IBuildContext>();
            restorePackages.Run(context);
        }
    }
}
