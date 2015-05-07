namespace Sitecore.Pathfinder.Parsing
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Models;

  [Export]
  public class ParseService
  {
    [ImportingConstructor]
    public ParseService([NotNull] ICompositionService compositionService, [NotNull] IFileSystemService fileSystemService)
    {
      this.CompositionService = compositionService;
      this.FileSystemService = fileSystemService;

      this.CompositionService.SatisfyImportsOnce(this);
    }

    [NotNull]
    public ICompositionService CompositionService { get; }

    [NotNull]
    public IFileSystemService FileSystemService { get; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    [NotNull]
    [ImportMany]
    public IEnumerable<IParser> Parsers { get; private set; }

    [NotNull]
    public virtual void Start([NotNull] IProject project)
    {
      // todo: change to abstract factory pattern
      var context = new ParseContext(this.CompositionService, this.FileSystemService, project);

      this.Start(context);
    }

    protected virtual void Start([NotNull] IParseContext context)
    {
      foreach (var parser in this.Parsers.OrderBy(c => c.Sortorder))
      {
        parser.Parse(context);
      }
    }
  }
}
