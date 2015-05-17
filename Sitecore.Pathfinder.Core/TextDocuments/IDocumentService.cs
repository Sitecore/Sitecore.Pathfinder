namespace Sitecore.Pathfinder.TextDocuments
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public interface IDocumentService
  {
    [NotNull]
    IDocument LoadDocument([NotNull] IProject project, [NotNull] ISourceFile sourceFile);

    [NotNull]
    string ReplaceTokens([NotNull] IProject project, [NotNull] ISourceFile sourceFile, [NotNull] string contents);
  }
}
