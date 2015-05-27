namespace Sitecore.Pathfinder.Documents
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public interface IDocumentLoader
  {
    bool CanLoad([NotNull] IDocumentService documentService, [NotNull] IProject project, [NotNull] ISourceFile sourceFile);

    [NotNull]
    ISnapshot Load([NotNull] IDocumentService documentService, [NotNull] IProject project, [NotNull] ISourceFile sourceFile);
  }
}
