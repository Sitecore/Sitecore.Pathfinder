// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.IO;
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

            var applicationExportProvider = new CatalogExportProvider(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            var coreExportProvider = new CatalogExportProvider(new AssemblyCatalog(typeof(ParseService).Assembly));
            var serverAssembliesExportProvider = new CatalogExportProvider(new DirectoryCatalog(directory, "Sitecore.Pathfinder.Server.*.dll"));
            var coreAssembliesExportProvider = new CatalogExportProvider(new DirectoryCatalog(directory, "Sitecore.Pathfinder.Core.*.dll"));

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
    }
}
