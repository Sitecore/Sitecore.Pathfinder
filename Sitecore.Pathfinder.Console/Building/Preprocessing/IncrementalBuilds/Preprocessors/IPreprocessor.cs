namespace Sitecore.Pathfinder.Building.Preprocessing.IncrementalBuilds.Preprocessors
{
  using Sitecore.Pathfinder.Diagnostics;

  public interface IPreprocessor
  {
    [NotNull]
    string Name { get; }

    void Execute([NotNull] IPreprocessingContext context, [NotNull] string fileName);
  }
}
