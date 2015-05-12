namespace Sitecore.Pathfinder.TreeNodes
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class TreeNode : ITreeNode
  {
    public static readonly ITreeNode Empty = new TreeNode(string.Empty, Pathfinder.TreeNodes.TextSpan.Empty);

    public TreeNode([NotNull] string name, [NotNull] ITextSpan textSpan, [CanBeNull] ITreeNode parent = null)
    {
      this.Name = name;
      this.TextSpan = textSpan;
      this.Parent = parent;
    }

    public string Name { get; }

    public IList<ITreeNode> TreeNodes { get; } = new List<ITreeNode>();

    public string GetAttributeValue(string attributeName)
    {
      return this.Attributes.FirstOrDefault(a => a.Name == attributeName)?.Value ?? string.Empty;
    }

    public IList<ITreeNodeAttribute> Attributes { get; } = new List<ITreeNodeAttribute>();

    public ITreeNode Parent { get; }

    public ITextSpan TextSpan { get; }
  }
}