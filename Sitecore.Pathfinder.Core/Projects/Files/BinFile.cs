namespace Sitecore.Pathfinder.Projects.Files
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TreeNodes;

  public class BinFile : File
  {
    public BinFile([NotNull] IProject project, [NotNull] ITextSpan textSpan) : base(project, textSpan)
    {
    }
  }
}
