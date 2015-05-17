namespace Sitecore.Pathfinder.Parsing
{
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.TextDocuments;

  public interface IParseContext
  {
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
    IParseContext With([NotNull] IProject project, [NotNull] IDocument document);
  }
}
