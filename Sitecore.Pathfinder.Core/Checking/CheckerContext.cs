namespace Sitecore.Pathfinder.Checking
{
  using System.ComponentModel.Composition;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Configuration;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(ICheckerContext))]
  public class CheckerContext : ICheckerContext
  {
    [ImportingConstructor]
    public CheckerContext([NotNull] IConfiguration configuration, [NotNull] IFactoryService factory)
    {
      this.Configuration = configuration;
      this.Factory = factory;

      this.IsDeployable = true;
    }

    public bool IsDeployable { get; set; }

    public IProject Project { get; private set; }

    public ITraceService Trace { get; private set; }

    [NotNull]
    protected IConfiguration Configuration { get; }

    [NotNull]
    protected IFactoryService Factory { get; }

    public ICheckerContext With(IProject project)
    {
      this.Project = project;
      this.Trace = new DiagnosticTraceService(this.Configuration, this.Factory).With(this.Project);

      return this;
    }
  }
}
