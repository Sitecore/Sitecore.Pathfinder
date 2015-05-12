namespace Sitecore.Pathfinder.Documents
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IDocumentService))]
  public class DocumentService : IDocumentService
  {
    [NotNull]
    [ImportMany]
    protected IEnumerable<IDocumentLexer> Loaders { get; private set; }

    public IDocument LoadDocument(IParseContext context, ISourceFile sourceFile)
    {
      foreach (var loader in this.Loaders)
      {
        if (loader.CanLex(context, sourceFile))
        {
          return loader.Lex(context, sourceFile);
        }
      }

      return new Document(sourceFile);
    }
  }
}