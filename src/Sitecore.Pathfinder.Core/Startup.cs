// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
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
        [NotNull]
        private static readonly Dictionary<string, IAppService> AppServiceCache = new Dictionary<string, IAppService>();

        public Startup()
        {
            ToolsDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            ProjectDirectory = Directory.GetCurrentDirectory();
            WebsiteDirectory = string.Empty;
            DataFolderDirectory = string.Empty;
        }

        [CanBeNull, ItemNotNull]
        public IEnumerable<string> AssemblyFileNames { get; private set; }

        public Extensibility.StartupExtensions.CompositionOptions CompositionOptions { get; private set; } = Extensibility.StartupExtensions.CompositionOptions.None;

        public ConfigurationOptions ConfigurationOptions { get; private set; } = ConfigurationOptions.Noninteractive;

        [NotNull]
        public string DataFolderDirectory { get; private set; }

        [NotNull]
        public string ProjectDirectory { get; private set; }

        [NotNull]
        public string ToolsDirectory { get; private set; }

        [NotNull]
        public string WebsiteDirectory { get; private set; }

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
        public Startup DoNotLoadConfigFiles()
        {
            ConfigurationOptions |= ConfigurationOptions.DoNotLoadConfig;
            return this;
        }

        [CanBeNull]
        public IAppService Start()
        {
            Console.WriteLine("App:1");
            IEnumerable<string> assemblyFileNames;
            if (AssemblyFileNames == null)
            {
                assemblyFileNames = Enumerable.Empty<string>();
            }
            else
            {
                assemblyFileNames = AssemblyFileNames.Distinct().OrderBy(a => a).ToList();
            }

            Console.WriteLine("App:2");
            var cacheKey = ToolsDirectory.ToLowerInvariant() + ProjectDirectory.ToLowerInvariant() + ConfigurationOptions + string.Join(",", assemblyFileNames);

            Console.WriteLine("App:3");
            IAppService app;
            if (AppServiceCache.TryGetValue(cacheKey, out app))
            {
                return app;
            }

            Console.WriteLine("App:4");
            var configuration = this.RegisterConfiguration(ToolsDirectory, ProjectDirectory, ConfigurationOptions);
            Console.WriteLine("App:5");
            if (configuration == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(WebsiteDirectory))
            {
                Console.WriteLine("App:6");
                configuration.Set(Constants.Configuration.WebsiteDirectory, WebsiteDirectory);
            }

            if (!string.IsNullOrEmpty(DataFolderDirectory))
            {
                Console.WriteLine("App:7");
                configuration.Set(Constants.Configuration.DataFolderDirectory, DataFolderDirectory);
            }

            Console.WriteLine("App:8");
            var compositionService = this.RegisterCompositionService(configuration, ProjectDirectory, Assembly.GetCallingAssembly(), assemblyFileNames, CompositionOptions);
            Console.WriteLine("App:9");
            if (compositionService == null)
            {
                return null;
            }

            Console.WriteLine("App:10");
            app = new AppService(configuration, compositionService, ToolsDirectory, ProjectDirectory);
            Console.WriteLine("App:11");
            AppServiceCache[cacheKey] = app;
            Console.WriteLine("App:12");
            return app;
        }

        [NotNull]
        public virtual Startup WithAssemblies([NotNull, ItemNotNull] IEnumerable<string> assemblyFileNames)
        {
            AssemblyFileNames = assemblyFileNames;
            return this;
        }

        [NotNull]
        public virtual Startup WithDataFolderDirectory([NotNull] string dataFolderDirectory)
        {
            DataFolderDirectory = dataFolderDirectory;
            return this;
        }

        [NotNull]
        public Startup WithExtensionsDirectory([NotNull] string directory)
        {
            var assemblyFileNames = Directory.Exists(directory) ? Directory.GetFiles(directory, "Sitecore.Pathfinder.*.dll") : Enumerable.Empty<string>();

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

        [NotNull]
        public Startup WithWebsiteDirectory([NotNull] string websiteDirectory)
        {
            WebsiteDirectory = websiteDirectory;
            return this;
        }
    }
}
