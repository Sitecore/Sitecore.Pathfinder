namespace Sitecore.Pathfinder.TextDocuments
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(ITextDocumentService))]
  public class TextDocumentService : ITextDocumentService
  {
    [NotNull]
    [ImportMany]
    protected IEnumerable<ITextDocumentLexer> Loaders { get; private set; }

    public ITextDocument LoadDocument(IParseContext context, ISourceFile sourceFile)
    {
      foreach (var loader in this.Loaders)
      {
        if (loader.CanLex(context, sourceFile))
        {
          return loader.Lex(context, sourceFile);
        }
      }

      return new TextDocument(sourceFile);
    }
  }
}