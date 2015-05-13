namespace Sitecore.Pathfinder.Documents
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public interface ITreeNode
  {
    [NotNull]
    IList<ITreeNode> Attributes { get; }

    [NotNull]
    IDocument Document { get; }

    int LineNumber { get; }

    int LinePosition { get; }

    [NotNull]
    string Name { get; }

    [CanBeNull]
    ITreeNode Parent { get; }

    [NotNull]
    IList<ITreeNode> TreeNodes { get; }

    [NotNull]
    string Value { get; }

    [NotNull]
    string GetAttributeValue([NotNull] string attributeName, [NotNull] string defaultValue = "");
  }
}
