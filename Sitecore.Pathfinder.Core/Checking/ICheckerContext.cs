namespace Sitecore.Pathfinder.Checking
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public interface ICheckerContext
  {
    bool IsDeployable { get; set; }

    [NotNull]
    IProject Project { get; }

    [NotNull]
    ITraceService Trace { get; }

    [NotNull]
    ICheckerContext With([NotNull] IProject project);
  }
}
