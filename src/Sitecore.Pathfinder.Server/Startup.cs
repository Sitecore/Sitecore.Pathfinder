// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.IO;
using Sitecore.Pathfinder.Emitters;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder
{
    public class Startup
    {
        [Diagnostics.NotNull]
        public virtual CompositionContainer RegisterCompositionService([Diagnostics.NotNull] IConfiguration configuration)
        {
            var compositionContainer = RegisterCompositionService();

            compositionContainer.ComposeExportedValue(configuration);

            return compositionContainer;
        }

        [Diagnostics.NotNull]
        public virtual CompositionContainer RegisterCompositionService()
        {
            var directory = FileUtil.MapPath("/bin");

            var conventions = new ExtensibilityConventions().GetConventions();

            var applicationExportProvider = new CatalogExportProvider(new AssemblyCatalog(Assembly.GetExecutingAssembly(), conventions));
            var coreExportProvider = new CatalogExportProvider(new AssemblyCatalog(typeof(ParseService).Assembly, conventions));
            var serverAssembliesExportProvider = new CatalogExportProvider(new DirectoryCatalog(directory, "Sitecore.Pathfinder.Server.*.dll", conventions));
            var coreAssembliesExportProvider = new CatalogExportProvider(new DirectoryCatalog(directory, "Sitecore.Pathfinder.Core.*.dll", conventions));

            // directory exports takes precedence over application exports
            var compositionContainer = new CompositionContainer(serverAssembliesExportProvider, coreAssembliesExportProvider, applicationExportProvider, coreExportProvider);

            applicationExportProvider.SourceProvider = compositionContainer;
            coreExportProvider.SourceProvider = compositionContainer;
            coreAssembliesExportProvider.SourceProvider = compositionContainer;
            serverAssembliesExportProvider.SourceProvider = compositionContainer;

            // register the composition service itself for DI
            compositionContainer.ComposeExportedValue<ICompositionService>(compositionContainer);

            return compositionContainer;
        }

        [Diagnostics.NotNull]
        public virtual IConfigurationSourceRoot RegisterConfiguration([Diagnostics.NotNull] string projectDirectory, EmitSource emitSource)
        {
            var configuration = new Microsoft.Framework.ConfigurationModel.Configuration();
            configuration.Add(new MemoryConfigurationSource());

            var toolsDirectory = emitSource == EmitSource.NugetPackage ? Path.Combine(projectDirectory, "content\\sitecore.tools") : Path.Combine(projectDirectory, "sitecore.tools");

            configuration.Set(Constants.Configuration.ToolsDirectory, toolsDirectory);
            configuration.Set(Constants.Configuration.SystemConfigFileName, "scconfig.json");

            return configuration;
        }
    }
}
