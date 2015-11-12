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
        [CanBeNull]
        public static CompositionContainer RegisterCompositionService([NotNull] this Startup startup, [NotNull] IConfiguration configuration, [NotNull] string projectDirectory, [NotNull] Assembly callingAssembly)
        {
            return RegisterCompositionService(startup, configuration, projectDirectory, callingAssembly, Enumerable.Empty<string>());
        }

        [CanBeNull]
        public static CompositionContainer RegisterCompositionService([NotNull] this Startup startup, [NotNull] IConfiguration configuration, [NotNull] string projectDirectory, [NotNull] Assembly callingAssembly, [NotNull][ItemNotNull] IEnumerable<string> additionalAssemblyFileNames)
        {
            var toolsDirectory = configuration.GetString(Constants.Configuration.ToolsDirectory);
            
            // add application assemblies
            var coreAssembly = typeof(Constants).Assembly;

            var catalogs = new List<ComposablePartCatalog>
            {
                new AssemblyCatalog(coreAssembly),
                new AssemblyCatalog(callingAssembly)
            };

            // add additional assemblies - this is used in Sitecore.Pathfinder.Server to load assemblies from the /bin folder
            foreach (var additionalAssemblyFileName in additionalAssemblyFileNames)
            {
                catalogs.Add(new AssemblyCatalog(additionalAssemblyFileName));
            }

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

            // build composition graph
            var exportProvider = new CatalogExportProvider(new AggregateCatalog(catalogs));
            var compositionContainer = new CompositionContainer(exportProvider);
            exportProvider.SourceProvider = compositionContainer;

            // register the composition service itself for DI
            compositionContainer.ComposeExportedValue<ICompositionService>(compositionContainer);
            compositionContainer.ComposeExportedValue(configuration);

            return compositionContainer;
        }

        private static void AddAssembliesFromDirectory([NotNull] [ItemNotNull] ICollection<ComposablePartCatalog> catalogs, [NotNull] string assemblyFileName, [NotNull] string directory)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            foreach (var fileName in Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories))
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

            var collectionsFileName = Path.Combine(toolsDirectory, "System.Collections.Immutable");
            if (!File.Exists(collectionsFileName))
            {
                return;
            }

            var codeAnalysisFileName = Path.Combine(toolsDirectory, "Microsoft.CodeAnalysis.dll");
            if (!File.Exists(codeAnalysisFileName))
            {
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
    }
}
