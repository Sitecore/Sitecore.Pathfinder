namespace Sitecore.Pathfinder.TextDocuments
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  public interface ITextDocumentLexer
  {
    bool CanLex([NotNull] IParseContext context, [NotNull] ISourceFile sourceFile);

    [NotNull]
    ITextDocument Lex([NotNull] IParseContext context, [NotNull] ISourceFile sourceFile);
  }
}