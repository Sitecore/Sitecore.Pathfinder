namespace Sitecore.Pathfinder.Documents
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
    ISnapshot Snapshot { get; }

    TextPosition Position { get; }

    [NotNull]
    string Name { get; }

    [CanBeNull]
    ITextNode Parent { get; }

    [NotNull]
    string Value { get; set; }

    [CanBeNull]
    ITextNode GetAttribute([NotNull] string attributeName);

    [NotNull]
    string GetAttributeValue([NotNull] string attributeName, [NotNull] string defaultValue = "");
  }
}
