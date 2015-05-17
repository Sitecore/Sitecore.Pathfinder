namespace Sitecore.Pathfinder.Projects.Files
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TextDocuments;

  public class BinFile : File
  {
    public BinFile([NotNull] IProject project, [NotNull] IDocument document) : base(project, document)
    {
    }
  }
}
