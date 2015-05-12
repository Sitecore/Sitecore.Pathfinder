namespace Sitecore.Pathfinder.Projects
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TreeNodes;

  public interface IProjectService
  {
    [NotNull]
    IDocument LoadDocument([NotNull] ISourceFile sourceFile);

    [NotNull]
    IProject LoadProject();
  }
}
