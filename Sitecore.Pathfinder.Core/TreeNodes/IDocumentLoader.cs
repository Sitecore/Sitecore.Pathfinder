namespace Sitecore.Pathfinder.TreeNodes
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public interface IDocumentLoader
  {
    bool CanLoad([NotNull] ISourceFile sourceFile);

    [NotNull]
    IDocument Load([NotNull] ISourceFile sourceFile);
  }
}
