namespace Sitecore.Pathfinder.Building.Preprocessing.IncrementalBuilds
{
  using Sitecore.Pathfinder.Diagnostics;

  public interface IPreprocessingContext
  {
    [NotNull]
    IBuildContext BuildContext { get; }

    [NotNull]
    string ContentDirectory { get; set; }

    [NotNull]
    string Database { get; set; }

    [NotNull]
    string ItemPath { get; set; }

    [NotNull]
    string SerializationDirectory { get; set; }
  }
}
