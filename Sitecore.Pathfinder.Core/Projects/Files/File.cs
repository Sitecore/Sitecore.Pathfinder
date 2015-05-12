namespace Sitecore.Pathfinder.Projects.Files
{
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  public class File : ProjectItem
  {
    private string qualifiedName;

    private string shortName;

    public File([NotNull] IProject project, [NotNull] ITreeNode treeNode) : base(project, treeNode)
    {
    }

    public override string QualifiedName => this.qualifiedName ?? (this.qualifiedName = this.TreeNode.Document.SourceFile.SourceFileName);

    public override string ShortName => this.shortName ?? (this.shortName = Path.GetFileName(this.TreeNode.Document.SourceFile.SourceFileName));

    public override void Analyze()
    {
      this.IsAnalyzed = true;
    }
  }
}
