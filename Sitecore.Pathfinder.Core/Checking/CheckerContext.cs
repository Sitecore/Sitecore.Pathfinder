namespace Sitecore.Pathfinder.Checking
{
  using System.ComponentModel.Composition;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(ICheckerContext))]
  public class CheckerContext : ICheckerContext
  {
    [ImportingConstructor]
    public CheckerContext([NotNull] IConfiguration configuration)
    {
      this.Configuration = configuration;

      this.IsDeployable = true;
    }

    public bool IsDeployable { get; set; }

    public IProject Project { get; private set; }

    public ITraceService Trace { get; private set; }

    [NotNull]
    protected IConfiguration Configuration { get; }

    public ICheckerContext With(IProject project)
    {
      this.Project = project;
      this.Trace = new DiagnosticTraceService(this.Configuration).With(this.Project);

      return this;
    }
  }
}
