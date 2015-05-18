namespace Sitecore.Pathfinder.Checking
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public interface ICheckerContext
  {
    [NotNull]
    IProject Project { get; }

    [NotNull]
    ITraceService Trace { get; }

    bool IsDeployable { get; set; }

    [NotNull]
    ICheckerContext With([NotNull] IProject project);
  }
}
