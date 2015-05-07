namespace Sitecore.Pathfinder.Models
{
  using Sitecore.Pathfinder.Diagnostics;

  public abstract class FileModelBase : ModelBase
  {
    protected FileModelBase([NotNull] string sourceFileName, [NotNull] string destinationFileName) : base(sourceFileName)
    {
      this.DestinationFileName = destinationFileName;
    }

    [NotNull]
    public string DestinationFileName { get; }
  }
}
