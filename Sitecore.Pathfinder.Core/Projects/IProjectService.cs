namespace Sitecore.Pathfinder.Projects
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TreeNodes;

  public interface IProjectService
  {
    [NotNull]
    IProject LoadProject();
  }
}
