namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  public class ExternalReferenceItem : ItemBase
  {
    public ExternalReferenceItem([NotNull] IProject project, [NotNull] ITreeNode treeNode) : base(project, treeNode)
    {
      this.IsEmittable = false;
    }

    public override void Analyze()
    {
      this.IsAnalyzed = true;
    }
  }
}
