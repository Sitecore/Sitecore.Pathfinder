namespace Sitecore.Pathfinder.Documents
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public class SnapshotTextNode : ITextNode
  {
    public SnapshotTextNode([NotNull] ISnapshot snapshot)
    {
      this.Snapshot = snapshot;
    }

    public IEnumerable<ITextNode> Attributes { get; } = new TextNode[0];

    public IEnumerable<ITextNode> ChildNodes { get; } = new TextNode[0];

    public string Name { get; } = string.Empty;

    public ITextNode Parent { get; } = null;

    public TextPosition Position { get; } = TextPosition.Empty;

    public ISnapshot Snapshot { get; }

    public string Value => this.Snapshot.SourceFile.GetFileNameWithoutExtensions();

    public ITextNode GetTextNodeAttribute(string attributeName)
    {
      return null;
    }

    public string GetAttributeValue(string attributeName, string defaultValue = "")
    {
      return string.Empty;
    }

    public bool SetValue(string value)
    {
      return false;
    }
  }
}
