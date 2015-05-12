namespace Sitecore.Pathfinder.Projects.Files
{
  using System.Diagnostics;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.TreeNodes;

  public class MediaFile : File
  {
    public MediaFile(IProject project, [NotNull] ITextSpan textSpan, [NotNull] Item mediaItem) : base(project, textSpan)
    {
      this.MediaItem = mediaItem;

      Debug.Assert(this.MediaItem.Owner == null, "Owner is already set");
      this.MediaItem.Owner = this;
    }

    [NotNull]
    public Item MediaItem { get; }
  }
}
