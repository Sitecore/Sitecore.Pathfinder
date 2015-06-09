// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Emitters
{
    public class EmitService
    {
        public EmitService([NotNull] string solutionDirectory)
        {
            SolutionDirectory = solutionDirectory;
        }

        [NotNull]
        public string SolutionDirectory { get; }

        public virtual void Start()
        {
            var configuration = RegisterConfiguration();
            var compositionService = RegisterCompositionService(configuration);

            var emitter = compositionService.GetExportedValue<Emitter>();
            emitter.Start();
        }

        [NotNull]
        protected virtual CompositionContainer RegisterCompositionService([Diagnostics.NotNull] IConfiguration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var directory = Path.GetDirectoryName(assembly.Location) ?? string.Empty;

            var applicationExportProvider = new CatalogExportProvider(new AssemblyCatalog(assembly));
            var coreExportProvider = new CatalogExportProvider(new AssemblyCatalog(typeof(ParseService).Assembly));
            var directoryExportProvider = new CatalogExportProvider(new DirectoryCatalog(directory, "Sitecore.Pathfinder.Server.*.dll"));

            // directory exports takes precedence over application exports
            var compositionContainer = new CompositionContainer(directoryExportProvider, applicationExportProvider, coreExportProvider);

            applicationExportProvider.SourceProvider = compositionContainer;
            coreExportProvider.SourceProvider = compositionContainer;
            directoryExportProvider.SourceProvider = compositionContainer;

            // register the composition service itself for DI
            compositionContainer.ComposeExportedValue<ICompositionService>(compositionContainer);
            compositionContainer.ComposeExportedValue(configuration);

            return compositionContainer;
        }

        [Diagnostics.NotNull]
        protected virtual IConfigurationSourceRoot RegisterConfiguration()
        {
            var configuration = new Microsoft.Framework.ConfigurationModel.Configuration();
            configuration.Add(new MemoryConfigurationSource());

            configuration.Set(Constants.Configuration.ToolsDirectory, Path.Combine(SolutionDirectory, "content\\.sitecore.tools"));
            configuration.Set(Constants.Configuration.SystemConfigFileName, "scconfig.json");

            return configuration;
        }
    }
}
