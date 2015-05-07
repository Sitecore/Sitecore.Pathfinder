namespace Sitecore.Pathfinder.Projects.MediaFiles
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Items;

  public class MediaFile : ItemBase
  {
    public MediaFile([NotNull] ISourceFile sourceFileName) : base(sourceFileName)
    {
    }
  }
}
