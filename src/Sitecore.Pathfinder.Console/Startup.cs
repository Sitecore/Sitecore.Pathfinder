// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Configuration;
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
            var errorCode = 0;

            var toolsDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            var projectDirectory = Directory.GetCurrentDirectory();

            var configuration = ConfigurationStartup.RegisterConfiguration(toolsDirectory, projectDirectory, ConfigurationOptions.Interactive);
            if (configuration == null)
            {
                errorCode = -1;
            }

            CompositionContainer compositionService = null;
            if (errorCode == 0 && configuration != null)
            {
                compositionService = RegisterCompositionService(configuration);
                if (compositionService == null)
                {
                    errorCode = -2;
                }
            }

            if (errorCode == 0 && compositionService != null)
            {
                var build = compositionService.Resolve<Build>();
                errorCode = build.Start();
            }

            if (configuration != null && string.Equals(configuration.Get("pause"), "true", StringComparison.OrdinalIgnoreCase))
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

            var coreExportProvider = new CatalogExportProvider(new AssemblyCatalog(typeof(Constants).Assembly));
            var applicationExportProvider = new CatalogExportProvider(new AssemblyCatalog(typeof(Startup).Assembly));
            var extensionsExportProvider = new CatalogExportProvider(new AssemblyCatalog(extensionsAssembly));

            // extensions directory exports takes precedence over application exports
            var compositionContainer = new CompositionContainer(extensionsExportProvider, applicationExportProvider, coreExportProvider);

            coreExportProvider.SourceProvider = compositionContainer;
            applicationExportProvider.SourceProvider = compositionContainer;
            extensionsExportProvider.SourceProvider = compositionContainer;

            // register the composition service itself for DI
            compositionContainer.ComposeExportedValue<ICompositionService>(compositionContainer);
            compositionContainer.ComposeExportedValue(configuration);

            return compositionContainer;
        }
    }
}
