namespace Sitecore.Pathfinder.Projects
{
  using Sitecore.Pathfinder.Diagnostics;

  public interface IProjectService
  {
    [NotNull]
    IProject LoadProjectFromConfiguration();
  }
}
