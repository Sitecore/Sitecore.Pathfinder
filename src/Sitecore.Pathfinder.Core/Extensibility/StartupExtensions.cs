// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Roslyn.Extensibility;

namespace Sitecore.Pathfinder.Extensibility
{
    public static class StartupExtensions
    {
        [Flags]
        public enum CompositionOptions
        {
            None = 0x0,

            AddWebsiteAssemblyResolver = 0x01,

            DisableExtensions = 0x02,

            IgnoreServerAssemblies = 0x04
        }

        [CanBeNull]
        public static CompositionContainer RegisterCompositionService([NotNull] this Startup startup, [NotNull] IConfiguration configuration, [NotNull] string projectDirectory, [NotNull, ItemNotNull] IEnumerable<string> additionalAssemblyFileNames, CompositionOptions options)
        {
            var toolsDirectory = configuration.GetToolsDirectory();

            if (options.HasFlag(CompositionOptions.AddWebsiteAssemblyResolver))
            {
                // add an assembly resolver that points to the website/bin directory - this will load files like Sitecore.Kernel.dll
                var websiteDirectory = configuration.GetWebsiteDirectory();
                if (!string.IsNullOrEmpty(websiteDirectory))
                {
                    AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => ResolveWebsiteAssembly(args, websiteDirectory);
                }
            }

            // add an assembly resolver for external assemblies
            var directories = configuration.GetDictionary(Constants.Configuration.Extensions.ExternalAssemblyDirectories);
            if (directories.Count > 0)
            {
                AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => ResolveAssembly(args, directories.Values);
            }

            // add application assemblies
            var coreAssembly = typeof(Constants).Assembly;

            var catalogs = new List<ComposablePartCatalog>
            {
                new AssemblyCatalog(coreAssembly)
            };

            var disableExtensions = configuration.GetBool(Constants.Configuration.DisableExtensions);
            if (!disableExtensions && !options.HasFlag(CompositionOptions.DisableExtensions))
            {
                // add additional assemblies - this is used in Sitecore.Pathfinder.Server to load assemblies from the /bin folder
                AddAdditionalAssemblies(catalogs, additionalAssemblyFileNames);

                // add core extensions - must come before feature assemblies to ensure the correct Sitecore.Pathfinder.Core.Extensions.dll is loaded
                var coreExtensionsDirectory = Path.Combine(toolsDirectory, "files\\extensions");
                var coreAssemblyFileName = Path.Combine(coreExtensionsDirectory, "Sitecore.Pathfinder.Core.Extensions.dll");

                AddDynamicAssembly(catalogs, toolsDirectory, coreAssemblyFileName, coreExtensionsDirectory);
                AddAssembliesFromDirectory(options, catalogs, coreExtensionsDirectory);

                // add feature assemblies from the same directory as Sitecore.Pathfinder.Core
                var binDirectory = configuration.GetString(Constants.Configuration.BinDirectory);
                if (string.IsNullOrEmpty(binDirectory))
                {
                    binDirectory = Path.GetDirectoryName(coreAssembly.Location);
                }

                if (!string.IsNullOrEmpty(binDirectory))
                {
                    AddFeatureAssemblies(options, catalogs, binDirectory);
                }

                // add extension from [Project]/packages directory
                AddNugetPackages(configuration, options, catalogs, coreAssemblyFileName, projectDirectory);

                // add extension from [Project]/node_modules directory
                AddNodeModules(configuration, options, catalogs, coreAssemblyFileName, projectDirectory);

                // add projects extensions
                var projectExtensionsDirectory = configuration.GetString(Constants.Configuration.Extensions.Directory);
                if (!string.IsNullOrEmpty(projectExtensionsDirectory))
                {
                    projectExtensionsDirectory = PathHelper.Combine(projectDirectory, projectExtensionsDirectory);
                    var projectAssemblyFileName = Path.Combine(projectExtensionsDirectory, configuration.GetString(Constants.Configuration.Extensions.AssemblyFileName));

                    AddDynamicAssembly(catalogs, toolsDirectory, projectAssemblyFileName, projectExtensionsDirectory);
                    AddAssembliesFromDirectory(options, catalogs, projectExtensionsDirectory);
                }
            }

            var isMultiThreaded = configuration.GetBool(Constants.Configuration.System.MultiThreaded);

            // build composition graph - thread-safe
            var exportProvider = new CatalogExportProvider(new AggregateCatalog(catalogs), isMultiThreaded);
            var compositionContainer = new CompositionContainer(exportProvider);
            exportProvider.SourceProvider = compositionContainer;

            // register the composition service itself for DI
            compositionContainer.ComposeExportedValue<ICompositionService>(compositionContainer);
            compositionContainer.ComposeExportedValue(configuration);

            return compositionContainer;
        }

        private static void AddAdditionalAssemblies([NotNull, ItemNotNull] ICollection<ComposablePartCatalog> catalogs, [NotNull, ItemNotNull] IEnumerable<string> additionalAssemblyFileNames)
        {
            foreach (var additionalAssemblyFileName in additionalAssemblyFileNames)
            {
                AddAssembly(catalogs, additionalAssemblyFileName);
            }
        }

        private static void AddAssembliesFromDirectory(CompositionOptions options, [NotNull, ItemNotNull] ICollection<ComposablePartCatalog> catalogs, [NotNull] string directory)
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

                // skip server assemblies, if interactive
                if ((options & CompositionOptions.IgnoreServerAssemblies) == CompositionOptions.IgnoreServerAssemblies && fileName.StartsWith("Sitecore.Pathfinder.Server.", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                AddAssembly(catalogs, assemblyFileName);
            }
        }

        private static void AddAssembly([NotNull, ItemNotNull] ICollection<ComposablePartCatalog> catalogs, [NotNull] string assemblyFileName)
        {
            var fileName = Path.GetFileName(assemblyFileName);

            if (catalogs.OfType<AssemblyCatalog>().Any(c => string.Equals(Path.GetFileName(c.Assembly.Location), fileName, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            try
            {
                var assemblyCatalog = new AssemblyCatalog(Assembly.LoadFrom(assemblyFileName));

                // check that assembly can be loaded
                if (assemblyCatalog.Any())
                {
                    catalogs.Add(assemblyCatalog);
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                // ignore the Diagnostics Toolset feature as it requires a path to the SDT installation
                if (fileName != "Sitecore.Pathfinder.DiagnosticsToolset.dll")
                {
                    Console.WriteLine(Texts.Failed_to_load_assembly___0____1_, ex.Message, assemblyFileName);

                    foreach (var loaderException in ex.LoaderExceptions)
                    {
                        Console.WriteLine($"    LoaderException: {loaderException.Message}");
                    }
                }
            }
            catch (FileLoadException ex)
            {
                Console.WriteLine(Texts.Failed_to_load_assembly___0____1_, ex.Message, assemblyFileName);
            }
        }

        private static void AddDynamicAssembly([NotNull, ItemNotNull] ICollection<ComposablePartCatalog> catalogs, [NotNull] string toolsDirectory, [NotNull] string assemblyFileName, [NotNull] string directory)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            var compilerService = new CsharpCompilerService();

            var fileNames = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);
            if (!fileNames.Any())
            {
                return;
            }

            var assembly = compilerService.Compile(toolsDirectory, assemblyFileName, fileNames);
            if (assembly != null)
            {
                AddAssembly(catalogs, assemblyFileName);
            }
        }

        private static void AddFeatureAssemblies(CompositionOptions options, [NotNull, ItemNotNull] ICollection<ComposablePartCatalog> catalogs, [NotNull] string toolsDirectory)
        {
            // only load Sitecore.Pathfinder.*.dll assemblies for performance
            foreach (var assemblyFileName in Directory.GetFiles(toolsDirectory, "Sitecore.Pathfinder.*.dll", SearchOption.TopDirectoryOnly))
            {
                var fileName = Path.GetFileName(assemblyFileName);
                if (string.IsNullOrEmpty(fileName))
                {
                    continue;
                }

                // skip server assemblies, if interactive
                if ((options & CompositionOptions.IgnoreServerAssemblies) == CompositionOptions.IgnoreServerAssemblies && fileName.StartsWith("Sitecore.Pathfinder.Server.", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                AddAssembly(catalogs, assemblyFileName);
            }
        }

        private static void AddNodeModules([NotNull] IConfiguration configuration, CompositionOptions options, [NotNull, ItemNotNull] ICollection<ComposablePartCatalog> catalogs, [NotNull] string coreAssemblyFileName, [NotNull] string projectDirectory)
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

                AddDynamicAssembly(catalogs, directory, coreAssemblyFileName, directory);
                AddAssembliesFromDirectory(options, catalogs, directory);
            }
        }

        private static void AddNugetPackages([NotNull] IConfiguration configuration, CompositionOptions options, [NotNull, ItemNotNull] ICollection<ComposablePartCatalog> catalogs, [NotNull] string coreAssemblyFileName, [NotNull] string projectDirectory)
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

        [CanBeNull]
        private static Assembly ResolveAssembly([NotNull] ResolveEventArgs args, [ItemNotNull, NotNull] IEnumerable<string> directories)
        {
            var fileName = args.Name;
            var n = fileName.IndexOf(',');
            if (n >= 0)
            {
                fileName = fileName.Left(n).Trim();
            }

            if (!fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".dll";
            }

            foreach (var directory in directories)
            {
                var assemblyFileName = Path.Combine(directory, fileName);
                if (File.Exists(assemblyFileName))
                {
                    return Assembly.LoadFrom(assemblyFileName);
                }
            }

            return null;
        }

        [CanBeNull]
        private static Assembly ResolveWebsiteAssembly([NotNull] ResolveEventArgs args, [NotNull] string websiteDirectory)
        {
            var fileName = args.Name;
            var n = fileName.IndexOf(',');
            if (n >= 0)
            {
                fileName = fileName.Left(n).Trim();
            }

            if (!fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".dll";
            }

            fileName = Path.Combine(websiteDirectory, "bin\\" + fileName);
            return File.Exists(fileName) ? Assembly.LoadFrom(fileName) : null;
        }
    }
}
