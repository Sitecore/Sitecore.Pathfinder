namespace Sitecore.Pathfinder.TextDocuments
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public interface ITextNode
  {
    [NotNull]
    IList<ITextNode> Attributes { get; }

    [NotNull]
    IList<ITextNode> ChildNodes { get; }

    [NotNull]
    IDocument Document { get; }

    int LineNumber { get; }

    int LinePosition { get; }

    [NotNull]
    string Name { get; }

    [CanBeNull]
    ITextNode Parent { get; }

    [NotNull]
    string Value { get; }

    [NotNull]
    string GetAttributeValue([NotNull] string attributeName, [NotNull] string defaultValue = "");
  }
}
