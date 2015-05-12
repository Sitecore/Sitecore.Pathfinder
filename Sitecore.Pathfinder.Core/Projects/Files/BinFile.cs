namespace Sitecore.Pathfinder.Projects.Files
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  public class BinFile : File
  {
    public BinFile([NotNull] IProject project, [NotNull] ITreeNode treeNode) : base(project, treeNode)
    {
    }
  }
}
