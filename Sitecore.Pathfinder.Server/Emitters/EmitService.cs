namespace Sitecore.Pathfinder.Emitters
{
  using System.ComponentModel.Composition;
  using System.ComponentModel.Composition.Hosting;
  using System.IO;
  using System.Reflection;
  using Sitecore.Pathfinder.Parsing;

  public class EmitService
  {
    public EmitService([NotNull] string projectDirectory)
    {
      this.ProjectDirectory = projectDirectory;
    }

    [NotNull]
    public string ProjectDirectory { get; }

    public virtual void Start()
    {
      var compositionService = this.RegisterCompositionService();

      var emitter = compositionService.GetExportedValue<Emitter>();
      emitter.Start(this.ProjectDirectory);
    }

    [NotNull]
    protected virtual CompositionContainer RegisterCompositionService()
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

      return compositionContainer;
    }
  }
}
