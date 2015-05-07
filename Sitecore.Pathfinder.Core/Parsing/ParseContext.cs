namespace Sitecore.Pathfinder.Parsing
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Models;

  public class ParseContext : IParseContext
  {
    public ParseContext([NotNull] ICompositionService compositionService, [NotNull] IFileSystemService fileSystemService, [NotNull] IProject project)
    {
      this.CompositionService = compositionService;
      this.FileSystem = fileSystemService;
      this.Project = project;
    }

    public ICompositionService CompositionService { get; }

    public IFileSystemService FileSystem { get; }

    public IProject Project { get; }
  }
}
