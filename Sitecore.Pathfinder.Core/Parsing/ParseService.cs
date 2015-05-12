namespace Sitecore.Pathfinder.Parsing
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IParseService))]
  public class ParseService : IParseService
  {
    [ImportingConstructor]
    public ParseService([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] IDocumentService documentService, [NotNull] ITokenService tokenService)
    {
      this.CompositionService = compositionService;
      this.Configuration = configuration;
      this.DocumentService = documentService;
      this.TokenService = tokenService;
    }

    [NotNull]
    [ImportMany]
    public IEnumerable<IParser> Parsers { get; private set; }

    [NotNull]
    public ITraceService TraceService { get; set; }

    [NotNull]
    protected ICompositionService CompositionService { get; }

    [NotNull]
    protected IConfiguration Configuration { get; }

    [NotNull]
    protected IDocumentService DocumentService { get; }

    [NotNull]
    protected ITokenService TokenService { get; }

    public virtual void Parse(IProject project, ISourceFile sourceFile)
    {
      // todo: change to abstract factory pattern
      var context = new ParseContext(this.CompositionService, this.Configuration, this.DocumentService, this.TokenService).Load(project, sourceFile);

      try
      {
        foreach (var parser in this.Parsers.OrderBy(c => c.Sortorder))
        {
          if (!parser.CanParse(context))
          {
            continue;
          }

          parser.Parse(context);
          break;
        }
      }
      catch (BuildException ex)
      {
        project.Trace.TraceError(Texts.Text3013, sourceFile.SourceFileName, ex.LineNumber, ex.LinePosition, ex.Message);
      }
      catch (Exception ex)
      {
        project.Trace.TraceError(Texts.Text3013, sourceFile.SourceFileName, 0, 0, ex.Message);
      }
    }
  }
}
