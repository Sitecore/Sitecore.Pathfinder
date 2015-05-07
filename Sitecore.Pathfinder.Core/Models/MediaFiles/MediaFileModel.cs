namespace Sitecore.Pathfinder.Models.MediaItems
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Models.Items;

  public class MediaFileModel : ItemModel
  {
    public MediaFileModel([NotNull] string sourceFileName) : base(sourceFileName)
    {
      this.MediaFile = string.Empty;
    }

    [NotNull]
    public string MediaFile { get; set; }
  }
}
