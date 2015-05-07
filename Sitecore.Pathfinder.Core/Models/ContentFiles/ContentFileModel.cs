namespace Sitecore.Pathfinder.Models.ContentFiles
{
  using Sitecore.Pathfinder.Diagnostics;

  public class ContentFileModel : FileModelBase
  {
    public ContentFileModel([NotNull] string sourceFileName, [NotNull] string destinationFileName) : base(sourceFileName, destinationFileName)
    {
    }
  }
}
