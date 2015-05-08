namespace Sitecore.Pathfinder.Parsing
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IParseService))]
  [PartCreationPolicy(CreationPolicy.Shared)]
  public class ParseService : IParseService
  {
    [ImportingConstructor]
    public ParseService([NotNull] ICompositionService compositionService)
    {
      this.CompositionService = compositionService;
    }

    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    [NotNull]
    [ImportMany]
    public IEnumerable<IParser> Parsers { get; private set; }

    [NotNull]
    protected ICompositionService CompositionService { get; }

    [NotNull]
    public virtual void Parse([NotNull] IProject project, [NotNull] ISourceFile sourceFile)
    {
      // todo: change to abstract factory pattern
      var context = new ParseContext(this.CompositionService, project, sourceFile);

      foreach (var parser in this.Parsers.OrderBy(c => c.Sortorder))
      {
        if (parser.CanParse(context))
        {
          parser.Parse(context);
        }
      }
    }
  }
}
