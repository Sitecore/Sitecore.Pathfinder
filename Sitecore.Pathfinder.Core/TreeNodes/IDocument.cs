namespace Sitecore.Pathfinder.TreeNodes
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public interface IDocument
  {
    [NotNull]
    ITreeNode Root { get; }

    [NotNull]
    ISourceFile SourceFile { get; }
  }
}