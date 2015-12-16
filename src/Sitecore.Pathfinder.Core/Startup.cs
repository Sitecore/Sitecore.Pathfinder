// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Framework.ConfigurationModel;
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

        [CanBeNull, ItemNotNull]
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

        [NotNull]
        public Startup DisableExtensions()
        {
            CompositionOptions |= Extensibility.StartupExtensions.CompositionOptions.DisableExtensions;
            return this;
        }

        [NotNull]
        private static readonly Dictionary<string, IAppService> AppServiceCache = new Dictionary<string, IAppService>();

        [CanBeNull]
        public IAppService Start()
        {
            var assemblyFileNames = AssemblyFileNames ?? Enumerable.Empty<string>();

            var cacheKey = ToolsDirectory.ToLowerInvariant() + ProjectDirectory.ToLowerInvariant() + ConfigurationOptions + string.Join(",", assemblyFileNames);

            IAppService app;
            if (AppServiceCache.TryGetValue(cacheKey, out app))
            {
                return app;
            }

            var configuration = this.RegisterConfiguration(ToolsDirectory, ProjectDirectory, ConfigurationOptions);
            if (configuration == null)
            {
                return null;
            }

            var compositionService = this.RegisterCompositionService(configuration, ProjectDirectory, Assembly.GetCallingAssembly(), assemblyFileNames, CompositionOptions);
            if (compositionService == null)
            {
                return null;
            }

            app = new AppService(configuration, compositionService, ToolsDirectory, ProjectDirectory);
            AppServiceCache[cacheKey] = app;
            return app;
        }

        [NotNull]
        public virtual Startup WithAssemblies([NotNull, ItemNotNull]  IEnumerable<string> assemblyFileNames)
        {
            AssemblyFileNames = assemblyFileNames;
            return this;
        }

        [NotNull]
        public Startup WithExtensionsDirectory([NotNull] string directory)
        {
            var assemblyFileNames = Directory.Exists(directory) ? Directory.GetFiles(directory, "Sitecore.Pathfinder.*.dll") : Enumerable.Empty<string>();

            // remove Sitecore.Pathfinder.Core and Sitecore.Pathfinder.Server assemblies
            assemblyFileNames = assemblyFileNames.Where(a => !string.Equals(Path.GetFileName(a), "Sitecore.Pathfinder.Core.dll", StringComparison.OrdinalIgnoreCase) && !string.Equals(Path.GetFileName(a), "Sitecore.Pathfinder.Server.dll", StringComparison.OrdinalIgnoreCase)).ToList();

            if (AssemblyFileNames == null)
            {
                AssemblyFileNames = assemblyFileNames;
            }
            else
            {
                AssemblyFileNames = AssemblyFileNames.Concat(assemblyFileNames).Distinct().ToList();
            }

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
