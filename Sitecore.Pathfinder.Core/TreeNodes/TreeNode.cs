namespace Sitecore.Pathfinder.TreeNodes
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public class TreeNode : ITreeNode
  {
    public TreeNode([NotNull] ITextSpan textSpan, [CanBeNull] ITreeNode parent = null)
    {
      this.TextSpan = textSpan;
      this.Parent = parent;
    }

    public IList<ITreeNode> Nodes { get; } = new List<ITreeNode>();

    public ITreeNode Parent { get; }

    public ITextSpan TextSpan { get; }
  }
}