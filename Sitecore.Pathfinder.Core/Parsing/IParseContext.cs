namespace Sitecore.Pathfinder.Parsing
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Models;

  public interface IParseContext
  {
    [NotNull]
    ICompositionService CompositionService { get; }

    [NotNull]
    IFileSystemService FileSystem { get; }

    [NotNull]
    IProject Project { get; }
  }
}
