namespace Sitecore.Pathfinder.Checking
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public interface ICheckerService
  {
    void CheckProject([NotNull] IProject project);
  }
}
