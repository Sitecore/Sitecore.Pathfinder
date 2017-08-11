// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

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
            ToolsDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? string.Empty;
        }

        [CanBeNull, ItemNotNull]
        public IEnumerable<string> AssemblyFileNames { get; private set; }

        [NotNull]
        public string BinDirectory { get; private set; } = string.Empty;

        [CanBeNull, ItemNotNull]
        public string[] CommandLine { get; private set; }

        public Extensibility.StartupExtensions.CompositionOptions CompositionOptions { get; private set; } = Extensibility.StartupExtensions.CompositionOptions.None;

        public ConfigurationOptions ConfigurationOptions { get; private set; } = ConfigurationOptions.Interactive;

        [NotNull]
        public string DataFolderDirectory { get; private set; } = string.Empty;

        [NotNull]
        public string PackageRootDirectory { get; private set; } = string.Empty;

        [NotNull]
        public string ProjectDirectory { get; private set; } = string.Empty;

        [CanBeNull]
        public Stopwatch Stopwatch { get; private set; }

        [NotNull]
        public string SystemConfigurationFileName { get; private set; } = "scconfig.json";

        [NotNull]
        public string ToolsDirectory { get; private set; }

        [NotNull]
        public string WebsiteDirectory { get; private set; } = string.Empty;

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
            if (string.IsNullOrEmpty(ProjectDirectory))
            {
                ProjectDirectory = GetProjectDirectory();
            }

            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetEntryAssembly();
            var assemblyFileNames = new List<string>
            {
                assembly.Location
            };

            if (AssemblyFileNames != null)
            {
                assemblyFileNames.AddRange(AssemblyFileNames.Distinct().OrderBy(a => a));
            }

            var configuration = this.RegisterConfiguration(ConfigurationOptions, ToolsDirectory, ProjectDirectory, SystemConfigurationFileName, CommandLine);
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

            if (!string.IsNullOrEmpty(BinDirectory))
            {
                configuration.Set(Constants.Configuration.BinDirectory, BinDirectory);
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
            var host = compositionService.Resolve<IHostService>().With(Stopwatch);

            // initialize extension - only called at start up
            foreach (var extension in compositionService.ResolveMany<IExtension>())
            {
                extension.Start();
            }

            return host;
        }

        [NotNull]
        public virtual Startup WithAssemblies([NotNull, ItemNotNull] IEnumerable<string> assemblyFileNames)
        {
            AssemblyFileNames = assemblyFileNames;
            return this;
        }

        [NotNull]
        public Startup WithBinDirectory([NotNull] string binDirectory)
        {
            BinDirectory = binDirectory;
            return this;
        }

        [NotNull]
        public Startup WithCommandLine([NotNull, ItemNotNull] string[] commandLine)
        {
            CommandLine = commandLine;
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
        public Startup WithWebsiteDirectory([NotNull] string websiteDirectory)
        {
            WebsiteDirectory = websiteDirectory;
            return this;
        }

        [NotNull]
        protected virtual string GetProjectDirectory()
        {
            // search in current and parent directories for scconfig.json
            var projectDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            do
            {
                var configurationFileName = Path.Combine(projectDirectory.FullName, SystemConfigurationFileName);
                if (File.Exists(configurationFileName))
                {
                    return projectDirectory.FullName;
                }

                projectDirectory = projectDirectory.Parent;
            }
            while (projectDirectory != null);

            return Directory.GetCurrentDirectory();
        }
    }
}
