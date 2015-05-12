namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  // todo: consider basing this on ProjectElement
  public class Field
  {
    public Field([NotNull] ITreeNode treeNode)
    {
      this.TreeNode = treeNode;
      this.Name = string.Empty;
      this.Value = string.Empty;
      this.Language = string.Empty;
    }

    public Field([NotNull] ITreeNode treeNode, [NotNull] string name, [NotNull] string value)
    {
      this.TreeNode = treeNode;
      this.Name = name;
      this.Value = value;
      this.Language = string.Empty;
    }

    [NotNull]
    public string Language { get; set; }

    [NotNull]
    public string Name { get; set; }

    [NotNull]
    public ITreeNode TreeNode { get; }

    [NotNull]
    public string Value { get; set; }

    public int Version { get; set; }
  }
}