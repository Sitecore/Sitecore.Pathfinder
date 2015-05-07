namespace Sitecore.Pathfinder.Models.SerializationFiles
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Models.Items;

  public class SerializationFileModel : ItemModel
  {
    public SerializationFileModel([NotNull] string sourceFileName) : base(sourceFileName)
    {
      this.SerializationFile = string.Empty;
    }

    [NotNull]
    public string SerializationFile { get; set; }
  }
}
