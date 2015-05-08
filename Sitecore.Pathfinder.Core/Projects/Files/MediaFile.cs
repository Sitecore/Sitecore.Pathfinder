namespace Sitecore.Pathfinder.Projects.Files
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Items;

  public class MediaFile : FileBase
  {
    public MediaFile([NotNull] ISourceFile sourceFileName, [NotNull] Item mediaItem) : base(sourceFileName)
    {
      this.MediaItem = mediaItem;
    }

    [NotNull]
    public Item MediaItem { get; }
  }
}
