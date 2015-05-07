namespace Sitecore.Pathfinder.Models.BinFiles
{
  using Sitecore.Pathfinder.Diagnostics;

  public class BinFileModel : FileModelBase
  {
    public BinFileModel([NotNull] string sourceFileName, [NotNull] string destinationFileName) : base(sourceFileName, destinationFileName)
    {
    }
  }
}
