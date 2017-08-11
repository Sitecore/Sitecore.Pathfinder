// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Extensibility
{
    public static class StartupExtensions
    {
        [Flags]
        public enum CompositionOptions
        {
            None = 0x0,

            DisableExtensions = 0x01
        }

        [CanBeNull]
        public static ICompositionService RegisterCompositionService([NotNull] this Startup startup, [NotNull] IConfiguration configuration, [NotNull] string projectDirectory, [NotNull, ItemNotNull] IEnumerable<string> additionalAssemblyFileNames, CompositionOptions options)
        {
            var toolsDirectory = configuration.GetToolsDirectory();

            // add application assemblies
            var coreAssembly = typeof(Constants).GetTypeInfo().Assembly;

            var assemblies = new List<Assembly>
            {
                coreAssembly,
                Assembly.GetEntryAssembly()
            };

            var disableExtensions = configuration.GetBool(Constants.Configuration.Extensions.Disabled);
            if (!disableExtensions && !options.HasFlag(CompositionOptions.DisableExtensions))
            {
                // add additional assemblies - this is used in Sitecore.Pathfinder.Server to load assemblies from the /bin folder
                AddAdditionalAssemblies(assemblies, additionalAssemblyFileNames);

                // add core extensions - must come before feature assemblies to ensure the correct Sitecore.Pathfinder.Core.Extensions.dll is loaded
                var coreExtensionsDirectory = Path.Combine(toolsDirectory, "files\\extensions");

                AddAssembliesFromDirectory(options, assemblies, coreExtensionsDirectory);

                // add feature assemblies from the same directory as Sitecore.Pathfinder.Core
                var binDirectory = configuration.GetString(Constants.Configuration.BinDirectory);
                if (string.IsNullOrEmpty(binDirectory))
                {
                    binDirectory = Path.GetDirectoryName(coreAssembly.Location);
                }

                if (!string.IsNullOrEmpty(binDirectory))
                {
                    AddFeatureAssemblies(options, assemblies, binDirectory);
                }

                // add extension from [Project]/packages directory
                AddNugetPackages(configuration, options, assemblies, projectDirectory);

                // add extension from [Project]/node_modules directory
                AddNodeModules(configuration, options, assemblies, projectDirectory);

                // add projects extensions
                var projectExtensionsDirectory = configuration.GetString(Constants.Configuration.Extensions.Directory);
                if (!string.IsNullOrEmpty(projectExtensionsDirectory))
                {
                    projectExtensionsDirectory = PathHelper.Combine(projectDirectory, projectExtensionsDirectory);

                    AddAssembliesFromDirectory(options, assemblies, projectExtensionsDirectory);
                }
            }

            var compositionHost = new ContainerConfiguration().WithProvider(new ConfigurationExportDescriptorProvider(configuration)).WithAssemblies(assemblies).CreateContainer();

            // todo: breaks DI
            var compositionService = ((CompositionService)compositionHost.GetExport<ICompositionService>()).With(compositionHost);

            return compositionService;
        }

        private static void AddAdditionalAssemblies([NotNull, ItemNotNull] ICollection<Assembly> catalogs, [NotNull, ItemNotNull] IEnumerable<string> additionalAssemblyFileNames)
        {
            foreach (var additionalAssemblyFileName in additionalAssemblyFileNames)
            {
                AddAssembly(catalogs, additionalAssemblyFileName);
            }
        }

        private static void AddAssembliesFromDirectory(CompositionOptions options, [NotNull, ItemNotNull] ICollection<Assembly> catalogs, [NotNull] string directory)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            // only load Sitecore.Pathfinder.*.dll assemblies for performance
            foreach (var assemblyFileName in Directory.GetFiles(directory, "Sitecore.Pathfinder.*.dll", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileName(assemblyFileName);
                if (string.IsNullOrEmpty(fileName))
                {
                    continue;
                }

                AddAssembly(catalogs, assemblyFileName);
            }
        }

        private static void AddAssembly([NotNull, ItemNotNull] ICollection<Assembly> assemblies, [NotNull] string assemblyFileName)
        {
            var fileName = Path.GetFileName(assemblyFileName);

            if (assemblies.Any(a => string.Equals(Path.GetFileName(a.Location), fileName, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            try
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFileName);
                assemblies.Add(assembly);
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine(Texts.Failed_to_load_assembly___0____1_, ex.Message, assemblyFileName);

                foreach (var loaderException in ex.LoaderExceptions)
                {
                    Console.WriteLine($"    LoaderException: {loaderException.Message}");
                }
            }
            catch (FileLoadException ex)
            {
                Console.WriteLine(Texts.Failed_to_load_assembly___0____1_, ex.Message, assemblyFileName);
            }
        }

        private static void AddFeatureAssemblies(CompositionOptions options, [NotNull, ItemNotNull] ICollection<Assembly> catalogs, [NotNull] string toolsDirectory)
        {
            // only load Sitecore.Pathfinder.*.dll assemblies for performance
            foreach (var assemblyFileName in Directory.GetFiles(toolsDirectory, "Sitecore.Pathfinder.*.dll", SearchOption.TopDirectoryOnly))
            {
                var fileName = Path.GetFileName(assemblyFileName);
                if (string.IsNullOrEmpty(fileName))
                {
                    continue;
                }

                AddAssembly(catalogs, assemblyFileName);
            }
        }

        private static void AddNodeModules([NotNull] IConfiguration configuration, CompositionOptions options, [NotNull, ItemNotNull] ICollection<Assembly> catalogs, [NotNull] string projectDirectory)
        {
            var nodeModules = Path.Combine(projectDirectory, configuration.GetString(Constants.Configuration.Packages.NpmDirectory));
            if (!Directory.Exists(nodeModules))
            {
                return;
            }

            foreach (var directory in Directory.GetDirectories(nodeModules))
            {
                var manifest = Path.Combine(directory, "pathfinder.json");
                if (!File.Exists(manifest))
                {
                    continue;
                }

                // todo: exclude nested node_modules directories

                AddAssembliesFromDirectory(options, catalogs, directory);
            }
        }

        private static void AddNugetPackages([NotNull] IConfiguration configuration, CompositionOptions options, [NotNull, ItemNotNull] ICollection<Assembly> catalogs, [NotNull] string projectDirectory)
        {
            // todo: consider only loading directories listed in packages.config or scconfig.json
            var nugetPackages = Path.Combine(projectDirectory, configuration.GetString(Constants.Configuration.Packages.NugetDirectory));
            if (!Directory.Exists(nugetPackages))
            {
                return;
            }

            foreach (var directory in Directory.GetDirectories(nugetPackages))
            {
                var manifest = Path.Combine(directory, "pathfinder.extension.manifest");
                if (!File.Exists(manifest))
                {
                    continue;
                }

                AddAssembliesFromDirectory(options, catalogs, directory);
            }
        }
    }

    internal class ConfigurationExportDescriptorProvider : ExportDescriptorProvider
    {
        [NotNull]
        private readonly IConfiguration _configuration;

        public ConfigurationExportDescriptorProvider([NotNull] IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [NotNull, ItemNotNull]
        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors([NotNull] CompositionContract contract, [NotNull] DependencyAccessor descriptorAccessor)
        {
            if (contract.ContractType == typeof(IConfiguration))
            {
                yield return new ExportDescriptorPromise(contract, contract.ContractType.FullName, true, NoDependencies, dependencies => ExportDescriptor.Create((context, operation) => _configuration, NoMetadata));
            }
        }
    }
}
