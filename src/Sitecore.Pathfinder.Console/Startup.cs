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
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder
{
    public class Startup
    {                          
        public virtual int Start()
        {
            var configuration = RegisterConfiguration();
            var compositionService = RegisterCompositionService(configuration);
            var errorCode = 0;

            if (compositionService != null)
            {
                var build = compositionService.Resolve<Build>();
                errorCode = build.Start();
            }

            if (string.Equals(configuration.Get("pause"), "true", StringComparison.OrdinalIgnoreCase))
            {
                Console.ReadLine();
            }

            return errorCode;
        }

        [CanBeNull]
        protected virtual CompositionContainer RegisterCompositionService([NotNull] IConfiguration configuration)
        {
            var extensionCompiler = new CsharpCompiler();

            var extensionsDirectory = Path.Combine(configuration.Get(Constants.Configuration.ToolsDirectory), "files\\extensions");
            var projectExtensionsDirectory = PathHelper.Combine(configuration.Get(Constants.Configuration.ToolsDirectory), "..\\sitecore.project\\extensions");
            var directories = new[]
            {
               extensionsDirectory,
               projectExtensionsDirectory
            };

            var extensionsAssembly = extensionCompiler.GetExtensionsAssembly(extensionsDirectory, directories);
            if (extensionsAssembly == null)
            {
                // todo: not nice
                return null;
            }

            var conventions = new ExtensibilityConventions().GetConventions();

            var applicationExportProvider = new CatalogExportProvider(new ApplicationCatalog(conventions));
            var extensionsExportProvider = new CatalogExportProvider(new AssemblyCatalog(extensionsAssembly, conventions));

            // extensions directory exports takes precedence over application exports
            var compositionContainer = new CompositionContainer(extensionsExportProvider, applicationExportProvider);

            applicationExportProvider.SourceProvider = compositionContainer;
            extensionsExportProvider.SourceProvider = compositionContainer;

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
            var projectDirectory = Directory.GetCurrentDirectory();

            configuration.Set(Constants.Configuration.ToolsDirectory, toolsDirectory);
            configuration.Set(Constants.Configuration.ProjectDirectory, projectDirectory);
            configuration.Set(Constants.Configuration.SystemConfigFileName, "scconfig.json");

            return configuration;
        }
    }
}
