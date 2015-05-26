namespace Sitecore.Pathfinder.Parsing
{
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects;

  public interface IParseContext
  {
    [NotNull]
    IConfiguration Configuration { get; }

    [NotNull]
    string DatabaseName { get; }

    [NotNull]
    IDocumentSnapshot DocumentSnapshot { get; }

    [NotNull]
    string ItemName { get; }

    [NotNull]
    string ItemPath { get; }

    [NotNull]
    string FilePath { get; }

    [NotNull]
    IProject Project { get; }

    [NotNull]
    ITraceService Trace { get; }

    [NotNull]
    IParseContext With([NotNull] IProject project, [NotNull] IDocumentSnapshot documentSnapshot);
  }
}
