namespace Sitecore.Pathfinder.Documents
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public interface ITextNode
  {
    [NotNull]
    IEnumerable<ITextNode> Attributes { get; }

    [NotNull]
    IEnumerable<ITextNode> ChildNodes { get; }

    [NotNull]
    string Name { get; }

    [CanBeNull]
    ITextNode Parent { get; }

    TextPosition Position { get; }

    [NotNull]
    ISnapshot Snapshot { get; }

    [NotNull]
    string Value { get; }

    [CanBeNull]
    ITextNode GetTextNodeAttribute([NotNull] string attributeName);

    [NotNull]
    string GetAttributeValue([NotNull] string attributeName, [NotNull] string defaultValue = "");

    bool SetValue([NotNull] string value);
  }
}
