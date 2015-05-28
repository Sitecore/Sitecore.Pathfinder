namespace Sitecore.Pathfinder.Parsing
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Extensions.CompositionServiceExtensions;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IParseService))]
  public class ParseService : IParseService
  {
    [ImportingConstructor]
    public ParseService([NotNull] ICompositionService compositionService, [NotNull] ISnapshotService snapshotService)
    {
      this.CompositionService = compositionService;
      this.SnapshotService = snapshotService;
    }

    [NotNull]
    [ImportMany]
    public IEnumerable<IParser> Parsers { get; private set; }

    [NotNull]
    protected ICompositionService CompositionService { get; }

    [NotNull]
    protected ISnapshotService SnapshotService { get; }

    public virtual void Parse(IProject project, ISourceFile sourceFile)
    {
      var textDocument = this.SnapshotService.LoadSnapshot(project, sourceFile);

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
      catch (Exception ex)
      {
        parseContext.Trace.TraceError(string.Empty, sourceFile.FileName, TextPosition.Empty, ex.Message);
      }
    }
  }
}
