namespace Sitecore.Pathfinder.Parsing
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IParseService))]
  public class ParseService : IParseService
  {
    [ImportingConstructor]
    public ParseService([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] ITokenService tokenService)
    {
      this.CompositionService = compositionService;
      this.Configuration = configuration;
      this.TokenService = tokenService;
    }

    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    [NotNull]
    [ImportMany]
    public IEnumerable<IParser> Parsers { get; private set; }

    [NotNull]
    protected ICompositionService CompositionService { get; }

    [NotNull]
    protected IConfiguration Configuration { get; }

    [NotNull]
    protected ITokenService TokenService { get; }

    public virtual void Parse(IProject project, ISourceFile sourceFile)
    {
      // todo: change to abstract factory pattern
      var context = new ParseContext(this.CompositionService, this.Configuration, this.TokenService).Load(project, sourceFile);

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
  }
}
