namespace Sitecore.Pathfinder.Checking
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(ICheckerContext))]
  public class CheckerContext : ICheckerContext
  {
    [ImportingConstructor]
    public CheckerContext([NotNull] ITraceService trace)
    {
      this.Trace = trace;

      this.IsDeployable = true;
    }

    public bool IsDeployable { get; set; }

    public IProject Project { get; private set; }

    public ITraceService Trace { get; }

    public ICheckerContext With(IProject project)
    {
      this.Project = project;

      return this;
    }
  }
}
