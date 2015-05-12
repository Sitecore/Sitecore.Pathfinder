namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  public class Item : ItemBase
  {
    public Item([NotNull] IProject project, [NotNull] ITreeNode treeNode) : base(project, treeNode)
    {
    }
  }
}
