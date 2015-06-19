// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder
{
    public class Startup
    {
        public virtual void Start()
        {
            var configuration = RegisterConfiguration();
            var compositionService = RegisterCompositionService(configuration);

            if (compositionService != null)
            {
                var build = compositionService.Resolve<Build>();
                build.Start();
            }

            if (string.Compare(configuration.Get("pause"), "true", StringComparison.OrdinalIgnoreCase) == 0)
            {
                Console.ReadLine();
            }
        }

        [CanBeNull]
        protected virtual CompositionContainer RegisterCompositionService([NotNull] IConfiguration configuration)
        {
            var toolspath = configuration.Get(Constants.Configuration.ToolsDirectory);

            var pluginDirectory = Path.Combine(toolspath, "plugins");
            Directory.CreateDirectory(pluginDirectory);

            var extensionCompiler = new CsharpCompiler();

            var extensionsDirectory = Path.Combine(configuration.Get(Constants.Configuration.ToolsDirectory), "files\\extensions");
            var extensionsAssembly = extensionCompiler.GetExtensionsAssembly(extensionsDirectory);
            if (extensionsAssembly == null)
            {
                // todo: not nice
                return null;
            }

            var applicationExportProvider = new CatalogExportProvider(new ApplicationCatalog());
            var extensionsExportProvider = new CatalogExportProvider(new AssemblyCatalog(extensionsAssembly));
            var pluginExportProvider = new CatalogExportProvider(new DirectoryCatalog(pluginDirectory));

            // plugin directory exports takes precedence over application exports
            var compositionContainer = new CompositionContainer(pluginExportProvider, extensionsExportProvider, applicationExportProvider);

            applicationExportProvider.SourceProvider = compositionContainer;
            extensionsExportProvider.SourceProvider = compositionContainer;
            pluginExportProvider.SourceProvider = compositionContainer;

            // register the composition service itself for DI
            compositionContainer.ComposeExportedValue<ICompositionService>(compositionContainer);
            compositionContainer.ComposeExportedValue(configuration);

            return compositionContainer;
        }

        [NotNull]
        protected virtual IConfigurationSourceRoot RegisterConfiguration()
        {
            var configuration = new Microsoft.Framework.ConfigurationModel.Configuration();
            configuration.Add(new MemoryConfigurationSource());

            var toolsDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

            configuration.Set(Constants.Configuration.ToolsDirectory, toolsDirectory);
            configuration.Set(Constants.Configuration.SystemConfigFileName, "scconfig.json");

            return configuration;
        }
    }
}
