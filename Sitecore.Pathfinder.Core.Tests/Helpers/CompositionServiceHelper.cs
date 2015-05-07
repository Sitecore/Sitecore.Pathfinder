namespace Sitecore.Pathfinder.Helpers
{
  using System.ComponentModel.Composition;
  using System.ComponentModel.Composition.Hosting;
  using System.IO;
  using System.Reflection;
  using Sitecore.Pathfinder.Diagnostics;

  public static class CompositionServiceHelper
  {
    [NotNull]
    public static CompositionContainer RegisterCompositionService()
    {
      var toolspath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

      var pluginDirectory = Path.Combine(toolspath, "plugins");
      Directory.CreateDirectory(pluginDirectory);

      var applicationExportProvider = new CatalogExportProvider(new ApplicationCatalog());
      var pluginExportProvider = new CatalogExportProvider(new DirectoryCatalog(pluginDirectory));

      // plugin directory exports takes precedence over application exports
      var compositionContainer = new CompositionContainer(pluginExportProvider, applicationExportProvider);

      applicationExportProvider.SourceProvider = compositionContainer;
      pluginExportProvider.SourceProvider = compositionContainer;

      // register the composition service itself for DI
      compositionContainer.ComposeExportedValue<ICompositionService>(compositionContainer);

      return compositionContainer;
    }
  }
}
