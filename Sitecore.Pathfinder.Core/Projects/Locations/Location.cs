namespace Sitecore.Pathfinder.Projects.Locations
{
  using Sitecore.Pathfinder.Diagnostics;

  public class Location
  {
    public Location([NotNull] ISourceFile sourceFile)
    {
      this.SourceFile = sourceFile;
    }

    [NotNull]
    public ISourceFile SourceFile { get; }
  }
}