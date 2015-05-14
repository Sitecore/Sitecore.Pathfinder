namespace Sitecore.Pathfinder.TextDocuments
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  public interface ITextDocumentService
  {
    [NotNull]
    ITextDocument LoadDocument([NotNull] IParseContext context, [NotNull] ISourceFile sourceFile);
  }
}