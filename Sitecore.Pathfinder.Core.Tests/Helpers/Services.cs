namespace Sitecore.Pathfinder.Helpers
{
  using System.ComponentModel.Composition;
  using System.ComponentModel.Composition.Hosting;
  using System.IO;
  using System.Reflection;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Configuration;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.CompositionServiceExtensions;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  public class Services
  {
    public CompositionContainer CompositionService { get; private set; }

    public IConfigurationSourceRoot Configuration { get; private set; }

    public IConfigurationService ConfigurationService { get; private set; }

    public IFileSystemService FileSystem { get; private set; }

    public IParseService ParseService { get; private set; }

    public IProjectService ProjectService { get; private set; }

    public ITraceService Trace { get; private set; }

    public CompositionContainer RegisterCompositionService([NotNull] IConfiguration configuration)
    {
      var toolspath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

      var pluginDirectory = Path.Combine(toolspath, "plugins");
      Directory.CreateDirectory(pluginDirectory);

      var coreExportProvider = new CatalogExportProvider(new AssemblyCatalog(typeof(Pathfinder.Constants).Assembly));
      var applicationExportProvider = new CatalogExportProvider(new AssemblyCatalog(typeof(Services).Assembly));
      var pluginExportProvider = new CatalogExportProvider(new DirectoryCatalog(pluginDirectory));

      // plugin directory exports takes precedence over application exports
      var compositionContainer = new CompositionContainer(pluginExportProvider, applicationExportProvider, coreExportProvider);

      coreExportProvider.SourceProvider = compositionContainer;
      applicationExportProvider.SourceProvider = compositionContainer;
      pluginExportProvider.SourceProvider = compositionContainer;

      // register the composition service itself for DI
      compositionContainer.ComposeExportedValue<ICompositionService>(compositionContainer);
      compositionContainer.ComposeExportedValue(configuration);

      return compositionContainer;
    }

    public void Start()
    {
      this.Configuration = this.RegisterConfiguration();
      this.CompositionService = this.RegisterCompositionService(this.Configuration);
      this.Trace = this.CompositionService.Resolve<ITraceService>();
      this.FileSystem = this.CompositionService.Resolve<IFileSystemService>();
      this.ParseService = this.CompositionService.Resolve<IParseService>();
      this.ProjectService = this.CompositionService.Resolve<IProjectService>();
      this.ConfigurationService = this.CompositionService.Resolve<IConfigurationService>();
    }

    [NotNull]
    protected IConfigurationSourceRoot RegisterConfiguration()
    {
      var configuration = new Configuration();
      configuration.Add(new MemoryConfigurationSource());

      var toolsDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "Website\\.sitecore.tools");
      configuration.Set(Pathfinder.Constants.ToolsDirectory, toolsDirectory);
      configuration.Set(Pathfinder.Constants.ConfigFileName, "sitecore.config.json");

      return configuration;
    }
  }
}