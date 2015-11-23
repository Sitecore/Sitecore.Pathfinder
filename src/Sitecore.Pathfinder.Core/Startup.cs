// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;

namespace Sitecore.Pathfinder
{
    public class Startup
    {
        public Startup()
        {
            ToolsDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            ProjectDirectory = Directory.GetCurrentDirectory();
        }

        [CanBeNull]
        [ItemNotNull]
        public IEnumerable<string> AssemblyFileNames { get; private set; }

        public Extensibility.StartupExtensions.CompositionOptions CompositionOptions { get; private set; } = Extensibility.StartupExtensions.CompositionOptions.None;

        public ConfigurationOptions ConfigurationOptions { get; private set; } = ConfigurationOptions.Noninteractive;

        [NotNull]
        public string ProjectDirectory { get; private set; }

        [NotNull]
        public string ToolsDirectory { get; private set; }

        [NotNull]
        public virtual Startup AsInteractive()
        {
            ConfigurationOptions = ConfigurationOptions.Interactive;
            return this;
        }

        [NotNull]
        public virtual Startup AsNoninteractive()
        {
            ConfigurationOptions = ConfigurationOptions.Noninteractive;
            return this;
        }

        public Startup DisableExtensions()
        {
            CompositionOptions |= Extensibility.StartupExtensions.CompositionOptions.DisableExtensions;
            return this;
        }

        [CanBeNull]
        public IAppService Start()
        {
            var configuration = this.RegisterConfiguration(ToolsDirectory, ProjectDirectory, ConfigurationOptions);
            if (configuration == null)
            {
                return null;
            }

            var assemblyFileNames = AssemblyFileNames ?? Enumerable.Empty<string>();

            var compositionService = this.RegisterCompositionService(configuration, ProjectDirectory, Assembly.GetCallingAssembly(), assemblyFileNames, CompositionOptions);
            if (compositionService == null)
            {
                return null;
            }

            return new AppService(configuration, compositionService, ToolsDirectory, ProjectDirectory);
        }

        [NotNull]
        public virtual Startup WithAssemblies([NotNull] [ItemNotNull] IEnumerable<string> assemblyFileNames)
        {
            AssemblyFileNames = assemblyFileNames;
            return this;
        }

        [NotNull]
        public virtual Startup WithProjectDirectory([NotNull] string projectDirectory)
        {
            ProjectDirectory = projectDirectory;
            return this;
        }

        [NotNull]
        public virtual Startup WithToolsDirectory([NotNull] string toolsDirectory)
        {
            ToolsDirectory = toolsDirectory;
            return this;
        }

        [NotNull]
        public virtual Startup WithWebsiteAssemblyResolver()
        {
            CompositionOptions |= Extensibility.StartupExtensions.CompositionOptions.AddWebsiteAssemblyResolver;
            return this;
        }
    }
}
