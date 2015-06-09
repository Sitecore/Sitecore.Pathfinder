namespace Sitecore.Pathfinder.Projects.Files
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.Items;

  public class MediaFile : File
  {
    public MediaFile([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] Item mediaItem) : base(project, snapshot)
    {
      this.MediaItem = mediaItem;
    }

    [NotNull]
    public Item MediaItem { get; }
  }
}
