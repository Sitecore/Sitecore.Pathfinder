namespace Sitecore.Pathfinder.Parsing
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.CompositionServiceExtensions;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.TextDocuments;

  [Export(typeof(IParseService))]
  public class ParseService : IParseService
  {
    [ImportingConstructor]
    public ParseService([NotNull] ICompositionService compositionService, [NotNull] IDocumentService documentService)
    {
      this.CompositionService = compositionService;
      this.DocumentService = documentService;
    }

    [NotNull]
    [ImportMany]
    public IEnumerable<IParser> Parsers { get; private set; }

    [NotNull]
    protected ICompositionService CompositionService { get; }

    [NotNull]
    protected IDocumentService DocumentService { get; }

    public virtual void Parse(IProject project, ISourceFile sourceFile)
    {
      var textDocument = this.DocumentService.LoadDocument(project, sourceFile);

      var parseContext = this.CompositionService.Resolve<IParseContext>().With(project, textDocument);

      try
      {
        foreach (var parser in this.Parsers.OrderBy(c => c.Sortorder))
        {
          if (parser.CanParse(parseContext))
          {
            parser.Parse(parseContext);
          }
        }
      }
      catch (BuildException ex)
      {
        parseContext.Trace.TraceError(Texts.Text3013, sourceFile.SourceFileName, ex.LineNumber, ex.LinePosition, ex.Message);
      }
      catch (Exception ex)
      {
        parseContext.Trace.TraceError(Texts.Text3013, sourceFile.SourceFileName, 0, 0, ex.Message);
      }
    }
  }
}
