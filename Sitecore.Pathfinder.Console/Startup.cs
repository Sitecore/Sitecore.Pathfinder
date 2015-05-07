namespace Sitecore.Pathfinder
{
  using System;
  using System.ComponentModel.Composition;
  using System.ComponentModel.Composition.Hosting;
  using System.Diagnostics;
  using System.IO;
  using System.Reflection;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Building;
  using Sitecore.Pathfinder.Diagnostics;

  public class Startup
  {
    public virtual void Start()
    {
      var configuration = this.RegisterConfiguration();
      var compositionService = this.RegisterCompositionService(configuration);

      var build = compositionService.GetExportedValue<Build>();
      build.Start();

      Trace.Write("Done");

      if (string.Compare(configuration.Get("pause"), "true", StringComparison.OrdinalIgnoreCase) == 0)
      {
        Console.ReadLine();
      }
    }

    [NotNull]
    protected virtual CompositionContainer RegisterCompositionService([NotNull] IConfiguration configuration)
    {
      var toolspath = configuration.Get(Building.Constants.ToolsPath);

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
      compositionContainer.ComposeExportedValue(configuration);

      return compositionContainer;
    }

    [NotNull]
    protected virtual IConfigurationSourceContainer RegisterConfiguration()
    {
      var configuration = new Configuration
      {
        new MemoryConfigurationSource()
      };

      var toolsPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

      configuration.Set(Building.Constants.ToolsPath, toolsPath);
      configuration.Set(Building.Constants.ConfigFileName, "sitecore.config.json");

      return configuration;
    }
  }
}
