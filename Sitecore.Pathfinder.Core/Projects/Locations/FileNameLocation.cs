namespace Sitecore.Pathfinder.Projects.Locations
{
  using Sitecore.Pathfinder.Diagnostics;

  public class FileNameLocation : Location
  {
    public FileNameLocation([NotNull] ISourceFile sourceFile) : base(sourceFile)
    {
    }
  }
}