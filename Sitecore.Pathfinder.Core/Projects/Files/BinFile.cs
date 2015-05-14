namespace Sitecore.Pathfinder.Projects.Files
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TextDocuments;

  public class BinFile : File
  {
    public BinFile([NotNull] IProject project, [NotNull] ITextNode textNode) : base(project, textNode)
    {
    }
  }
}
