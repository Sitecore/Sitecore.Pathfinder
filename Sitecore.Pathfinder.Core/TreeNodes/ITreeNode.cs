namespace Sitecore.Pathfinder.TreeNodes
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public interface ITreeNode
  {
    [NotNull]
    IList<ITreeNodeAttribute> Attributes { get; }

    [NotNull]
    string Name { get; }

    [CanBeNull]
    ITreeNode Parent { get; }

    [NotNull]
    ITextSpan TextSpan { get; }

    [NotNull]
    IList<ITreeNode> TreeNodes { get; }

    [NotNull]
    string GetAttributeValue([NotNull] string attributeName);
  }
}