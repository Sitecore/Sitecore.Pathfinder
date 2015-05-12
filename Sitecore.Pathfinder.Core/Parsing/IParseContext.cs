namespace Sitecore.Pathfinder.Parsing
{
  using System.ComponentModel.Composition;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.TreeNodes;

  public interface IParseContext
  {
    [NotNull]
    ICompositionService CompositionService { get; }

    [NotNull]
    IConfiguration Configuration { get; }

    [NotNull]
    string DatabaseName { get; }

    [NotNull]
    string ItemName { get; }

    [NotNull]
    string ItemPath { get; }

    [NotNull]
    IProject Project { get; }

    [NotNull]
    IDocument Document { get; }

    [NotNull]
    string GetRelativeFileName([NotNull] ISourceFile sourceFile);

    [NotNull]
    IParseContext Load([NotNull] IProject project, [NotNull] ISourceFile sourceFile);

    [NotNull]
    string ReplaceTokens([NotNull] string text);
  }
}
