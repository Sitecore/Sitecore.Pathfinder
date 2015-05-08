namespace Sitecore.Pathfinder.Parsing
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public interface IParseContext
  {
    [NotNull]
    ICompositionService CompositionService { get; }

    [NotNull]
    string DatabaseName { get; }

    [NotNull]
    string ItemName { get; }

    [NotNull]
    string ItemPath { get; }

    [NotNull]
    IProject Project { get; }

    [NotNull]
    ISourceFile SourceFile { get; }

    [NotNull]
    string GetRelativeFileName([NotNull] ISourceFile sourceFile);
  }
}
