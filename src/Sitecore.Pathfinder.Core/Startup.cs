// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Extensions;

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

        [NotNull]
        public string Configuration { get; private set; }

        public ConfigurationOptions ConfigurationOptions { get; private set; } = ConfigurationOptions.Noninteractive;

        [NotNull]
        public string DataFolderDirectory { get; private set; } = string.Empty;

        [NotNull]
        public string PackageRootDirectory { get; private set; } = string.Empty;

        [NotNull]
        public string ProjectDirectory { get; private set; }

        [CanBeNull]
        public Stopwatch Stopwatch { get; private set; }

        [NotNull]
        public string SystemConfigurationFileName { get; private set; } = "scconfig.json";

        [NotNull]
        public string ToolsDirectory { get; private set; }

        [NotNull]
        public string WebsiteDirectory { get; private set; } = string.Empty;

        [NotNull]
        public virtual Startup AsInteractive()
        {
            ConfigurationOptions = ConfigurationOptions.Interactive;
            CompositionOptions |= Extensibility.StartupExtensions.CompositionOptions.IgnoreServerAssemblies;
            return this;
        }

        [NotNull]
        public virtual Startup AsNoninteractive()
        {
            ConfigurationOptions = ConfigurationOptions.Noninteractive;
            CompositionOptions &= ~Extensibility.StartupExtensions.CompositionOptions.IgnoreServerAssemblies;
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
        public IHostService Start()
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            var assemblyFileNames = new List<string>
            {
                assembly.Location
            };

            if (AssemblyFileNames != null)
            {
                assemblyFileNames.AddRange(AssemblyFileNames.Distinct().OrderBy(a => a));
            }

            var configuration = this.RegisterConfiguration(ToolsDirectory, ProjectDirectory, SystemConfigurationFileName, ConfigurationOptions);
            if (configuration == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(WebsiteDirectory))
            {
                configuration.Set(Constants.Configuration.WebsiteDirectory, WebsiteDirectory);
            }

            if (!string.IsNullOrEmpty(DataFolderDirectory))
            {
                configuration.Set(Constants.Configuration.DataFolderDirectory, DataFolderDirectory);
            }

            if (!string.IsNullOrEmpty(PackageRootDirectory))
            {
                configuration.Set(Constants.Configuration.NugetPackageRootDirectory, PackageRootDirectory);
            }
            else
            {
                configuration.Set(Constants.Configuration.NugetPackageRootDirectory, Path.Combine(ProjectDirectory, configuration.GetString(Constants.Configuration.Packages.NugetDirectory)));
            }

            var compositionService = this.RegisterCompositionService(configuration, ProjectDirectory, assemblyFileNames, CompositionOptions);
            if (compositionService == null)
            {
                return null;
            }

            // create the host
            var host = new HostService(configuration, compositionService, Stopwatch);
            compositionService.Set((IHostService)host);

            return host;
        }

        [NotNull]
        public virtual Startup WithAssemblies([NotNull, ItemNotNull] IEnumerable<string> assemblyFileNames)
        {
            AssemblyFileNames = assemblyFileNames;
            return this;
        }

        [NotNull]
        public Startup WithConfigurationOptions(ConfigurationOptions options)
        {
            ConfigurationOptions = options;
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
        public Startup WithPackageRootDirectory([NotNull] string packageRootDirectory)
        {
            PackageRootDirectory = packageRootDirectory;
            return this;
        }

        [NotNull]
        public virtual Startup WithProjectDirectory([NotNull] string projectDirectory)
        {
            ProjectDirectory = projectDirectory;
            return this;
        }

        [NotNull]
        public Startup WithStopWatch()
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();

            return this;
        }

        [NotNull]
        public virtual Startup WithSystemConfigurationFileName([NotNull] string systemConfigurationFileName)
        {
            SystemConfigurationFileName = systemConfigurationFileName;
            return this;
        }

        [NotNull]
        public virtual Startup WithToolsDirectory([NotNull] string toolsDirectory)
        {
            ToolsDirectory = toolsDirectory;
            return this;
        }

        [NotNull]
        public Startup WithTraceListeners()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
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
