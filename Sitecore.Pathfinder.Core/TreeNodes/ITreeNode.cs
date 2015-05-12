namespace Sitecore.Pathfinder.TreeNodes
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public interface ITreeNode
  {
    [NotNull]
    IList<ITreeNode> Nodes { get; }

    [CanBeNull]
    ITreeNode Parent { get; }

    [NotNull]
    ITextSpan TextSpan { get; }
  }
}