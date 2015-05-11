namespace Sitecore.Pathfinder.Projects.Files
{
  using Sitecore.Pathfinder.Diagnostics;

  public class BinFile : File
  {
    public BinFile([NotNull] IProject project, [NotNull] ISourceFile sourceFile) : base(project, sourceFile)
    {
    }
  }
}
