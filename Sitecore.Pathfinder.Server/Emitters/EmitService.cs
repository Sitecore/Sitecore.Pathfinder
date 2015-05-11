namespace Sitecore.Pathfinder.Emitters
{
  using System.ComponentModel.Composition;
  using System.ComponentModel.Composition.Hosting;
  using System.IO;
  using System.Reflection;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;

  public class EmitService
  {
    public EmitService([Sitecore.NotNull] string solutionDirectory)
    {
      this.SolutionDirectory = solutionDirectory;
    }

    [Sitecore.NotNull]
    public string SolutionDirectory { get; }

    public virtual void Start()
    {
      var configuration = this.RegisterConfiguration();
      var compositionService = this.RegisterCompositionService(configuration);

      var emitter = compositionService.GetExportedValue<Emitter>();
      emitter.Start();
    }

    [Sitecore.NotNull]
    protected virtual CompositionContainer RegisterCompositionService([NotNull] IConfiguration configuration)
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

    [NotNull]
    protected virtual IConfigurationSourceRoot RegisterConfiguration()
    {
      var configuration = new Configuration();
      configuration.Add(new MemoryConfigurationSource());

      configuration.Set(Pathfinder.Constants.ToolsDirectory, Path.Combine(this.SolutionDirectory, "content\\.sitecore.tools"));
      configuration.Set(Pathfinder.Constants.ConfigFileName, "sitecore.config.json");

      return configuration;
    }
  }
}
