namespace Sitecore.Pathfinder.Projects.Locations
{
  using Sitecore.Pathfinder.Diagnostics;

  public class SerializationFileLocation : Location
  {
    public SerializationFileLocation([NotNull] ISourceFile sourceFile, int startIndex) : base(sourceFile)
    {
      this.StartIndex = startIndex;
    }

    public int StartIndex { get; }
  }
}