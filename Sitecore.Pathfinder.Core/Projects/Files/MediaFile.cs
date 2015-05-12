namespace Sitecore.Pathfinder.Projects.Files
{
  using System.Diagnostics;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.Items;

  public class MediaFile : File
  {
    public MediaFile(IProject project, [NotNull] ITreeNode treeNode, [NotNull] Item mediaItem) : base(project, treeNode)
    {
      this.MediaItem = mediaItem;

      Debug.Assert(this.MediaItem.Owner == null, "Owner is already set");
      this.MediaItem.Owner = this;
    }

    [NotNull]
    public Item MediaItem { get; }
  }
}
