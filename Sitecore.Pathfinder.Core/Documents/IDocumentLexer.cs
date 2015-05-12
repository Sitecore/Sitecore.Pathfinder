namespace Sitecore.Pathfinder.Documents
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  public interface IDocumentLexer
  {
    bool CanLex(IParseContext context, [NotNull] ISourceFile sourceFile);

    [NotNull]
    IDocument Lex(IParseContext context, [NotNull] ISourceFile sourceFile);
  }
}