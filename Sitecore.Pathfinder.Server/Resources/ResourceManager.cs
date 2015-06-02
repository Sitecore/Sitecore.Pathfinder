namespace Sitecore.Pathfinder.Resources
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.ComponentModel.Composition.Hosting;
  using System.IO;
  using System.Reflection;
  using Sitecore.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Zip;

  public class ResourceManager
  {
    public ResourceManager()
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

      compositionContainer.SatisfyImportsOnce(this);
    }

    [NotNull]
    [UsedImplicitly]
    [ImportMany(typeof(IResourceExporter))]
    public IEnumerable<IResourceExporter> Exporters { get; protected set; }

    [NotNull]
    public string BuildResourceFile()
    {
      TempFolder.EnsureFolder();

      var fileName = FileUtil.MapPath(TempFolder.GetFilename("Pathfinder.Resources.zip"));
      using (var zip = new ZipWriter(fileName))
      {
        foreach (var exporter in this.Exporters)
        {
          exporter.Export(zip);
        }
      }

      return fileName;
    }
  }
}