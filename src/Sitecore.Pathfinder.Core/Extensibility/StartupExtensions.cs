// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Framework.ConfigurationModel;
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

            AddWebsiteAssemblyResolver = 0x01,

            DisableExtensions = 0x2
        }

        [CanBeNull]
        public static CompositionContainer RegisterCompositionService([NotNull] this Startup startup, [NotNull] IConfiguration configuration, [NotNull] string projectDirectory, [NotNull] Assembly callingAssembly, [NotNull] [ItemNotNull] IEnumerable<string> additionalAssemblyFileNames, CompositionOptions options)
        {
            var toolsDirectory = configuration.GetString(Constants.Configuration.ToolsDirectory);

            if (options.HasFlag(CompositionOptions.AddWebsiteAssemblyResolver))
            {
                // add an assembly resolver that points to the website/bin directory - this will load files like Sitecore.Kernel.dll
                AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => ResolveAssembly(args, configuration);
            }

            // add application assemblies
            var coreAssembly = typeof(Constants).Assembly;

            var catalogs = new List<ComposablePartCatalog>
            {
                new AssemblyCatalog(coreAssembly),
                new AssemblyCatalog(callingAssembly)
            };

            var disableExtensions = configuration.GetBool("disable-extensions");
            if (!disableExtensions && !options.HasFlag(CompositionOptions.DisableExtensions))
            {
                // add assemblies from the tools directory
                AddFeatureAssemblies(catalogs, toolsDirectory);

                // add additional assemblies - this is used in Sitecore.Pathfinder.Server to load assemblies from the /bin folder
                AddAdditionalAssemblies(catalogs, additionalAssemblyFileNames);

                // add core extensions
                var coreExtensionsDirectory = Path.Combine(toolsDirectory, "files\\extensions");
                var coreAssemblyFileName = Path.Combine(coreExtensionsDirectory, "Sitecore.Pathfinder.Core.Extensions.dll");
                AddAssembliesFromDirectory(catalogs, coreAssemblyFileName, coreExtensionsDirectory);
                AddDynamicAssembly(catalogs, toolsDirectory, coreAssemblyFileName, coreExtensionsDirectory);

                // add projects extensions
                var projectExtensionsDirectory = PathHelper.Combine(projectDirectory, configuration.GetString(Constants.Configuration.ProjectExtensionsDirectory));
                var projectAssemblyFileName = Path.Combine(projectExtensionsDirectory, configuration.GetString(Constants.Configuration.ProjectExtensionsAssemblyFileName));
                AddAssembliesFromDirectory(catalogs, projectAssemblyFileName, projectExtensionsDirectory);
                AddDynamicAssembly(catalogs, toolsDirectory, projectAssemblyFileName, projectExtensionsDirectory);
            }

            // build composition graph
            var exportProvider = new CatalogExportProvider(new AggregateCatalog(catalogs));
            var compositionContainer = new CompositionContainer(exportProvider);
            exportProvider.SourceProvider = compositionContainer;

            // register the composition service itself for DI
            compositionContainer.ComposeExportedValue<ICompositionService>(compositionContainer);
            compositionContainer.ComposeExportedValue(configuration);

            return compositionContainer;
        }

        private static void AddAdditionalAssemblies([NotNull] [ItemNotNull] ICollection<ComposablePartCatalog> catalogs, [NotNull] [ItemNotNull] IEnumerable<string> additionalAssemblyFileNames)
        {
            foreach (var additionalAssemblyFileName in additionalAssemblyFileNames)
            {
                catalogs.Add(new AssemblyCatalog(additionalAssemblyFileName));
            }
        }

        private static void AddAssembliesFromDirectory([NotNull] [ItemNotNull] ICollection<ComposablePartCatalog> catalogs, [NotNull] string assemblyFileName, [NotNull] string directory)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            foreach (var fileName in Directory.GetFiles(directory, "Sitecore.Pathfinder.*.dll", SearchOption.AllDirectories))
            {
                if (!string.Equals(fileName, assemblyFileName, StringComparison.OrdinalIgnoreCase))
                {
                    catalogs.Add(new AssemblyCatalog(fileName));
                }
            }
        }

        private static void AddDynamicAssembly([NotNull] [ItemNotNull] ICollection<ComposablePartCatalog> catalogs, [NotNull] string toolsDirectory, [NotNull] string assemblyFileName, [NotNull] string directory)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            var collectionsFileName = Path.Combine(toolsDirectory, "System.Collections.Immutable.dll");
            if (!File.Exists(collectionsFileName))
            {
                Console.WriteLine("System.Collections.Immutable.dll is missing. Extensions will not be loaded.");
                return;
            }

            var codeAnalysisFileName = Path.Combine(toolsDirectory, "Microsoft.CodeAnalysis.dll");
            if (!File.Exists(codeAnalysisFileName))
            {
                Console.WriteLine("Microsoft.CodeAnalysis.dll is missing. Extensions will not be loaded.");
                return;
            }

            var compilerService = new CsharpCompilerService();

            var fileNames = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);
            if (!fileNames.Any())
            {
                return;
            }

            var assembly = compilerService.Compile(assemblyFileName, fileNames);
            if (assembly != null)
            {
                catalogs.Add(new AssemblyCatalog(assembly));
            }
        }

        private static void AddFeatureAssemblies([NotNull] [ItemNotNull] ICollection<ComposablePartCatalog> catalogs, [NotNull] string toolsDirectory)
        {
            foreach (var fileName in Directory.GetFiles(toolsDirectory, "Sitecore.Pathfinder.*.dll", SearchOption.TopDirectoryOnly))
            {
                // already added
                if (string.Equals(Path.GetFileName(fileName), "Sitecore.Pathfinder.Core.dll", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                catalogs.Add(new AssemblyCatalog(fileName));
            }
        }

        [CanBeNull]
        private static Assembly ResolveAssembly([NotNull] ResolveEventArgs args, [NotNull] IConfiguration configuration)
        {
            var websiteDirectory = configuration.GetString(Constants.Configuration.WebsiteDirectory);

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
