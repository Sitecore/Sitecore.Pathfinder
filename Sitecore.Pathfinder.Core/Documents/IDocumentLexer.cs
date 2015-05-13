namespace Sitecore.Pathfinder.Documents
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  public interface IDocumentLexer
  {
    bool CanLex([NotNull] IParseContext context, [NotNull] ISourceFile sourceFile);

    [NotNull]
    IDocument Lex([NotNull] IParseContext context, [NotNull] ISourceFile sourceFile);
  }
}