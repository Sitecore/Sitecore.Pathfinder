namespace Sitecore.Pathfinder.Building.Preprocessing.IncrementalBuilds
{
  using Sitecore.Pathfinder.Diagnostics;

  public class PreprocessingContext : IPreprocessingContext
  {
    public PreprocessingContext([NotNull] IBuildContext buildContext)
    {
      this.BuildContext = buildContext;
      this.Database = string.Empty;
    }

    public IBuildContext BuildContext { get; }

    public string ContentDirectory { get; set; }

    public string Database { get; set; }

    public string ItemPath { get; set; }

    public string SerializationDirectory { get; set; }
  }
}
