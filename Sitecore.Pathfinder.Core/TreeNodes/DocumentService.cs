namespace Sitecore.Pathfinder.TreeNodes
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
    protected IEnumerable<IDocumentLoader> Loaders { get; private set; }

    public IDocument LoadDocument(IParseContext context, ISourceFile sourceFile)
    {
      foreach (var loader in this.Loaders)
      {
        if (loader.CanLoad(context, sourceFile))
        {
          return loader.Load(context, sourceFile);
        }
      }

      return new Document(sourceFile);
    }
  }
}