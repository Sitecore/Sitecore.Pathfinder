namespace Sitecore.Pathfinder.Projects
{
  using Sitecore.Pathfinder.Diagnostics;

  public interface IProjectService
  {
    [NotNull]
    IProject LoadProject([NotNull] string projectDirectory, [NotNull] string databaseName, [NotNull] string[] ignoreDirectories);
  }
}
