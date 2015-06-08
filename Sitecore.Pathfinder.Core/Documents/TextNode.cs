namespace Sitecore.Pathfinder.Documents
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class TextNode : ITextNode
  {
    public static readonly ITextNode Empty = new SnapshotTextNode(Documents.Snapshot.Empty);

    public TextNode([NotNull] ISnapshot snapshot, TextPosition position, [NotNull] string name, [NotNull] string value, [CanBeNull] ITextNode parent)
    {
      this.Snapshot = snapshot;
      this.Position = position;
      this.Name = name;
      this.Value = value;
      this.Parent = parent;
    }

    public IEnumerable<ITextNode> Attributes { get; } = new List<ITextNode>();

    public IEnumerable<ITextNode> ChildNodes { get; } = new List<ITextNode>();

    public string Name { get; }

    public ITextNode Parent { get; }

    public TextPosition Position { get; }

    public ISnapshot Snapshot { get; }

    public string Value { get; protected set; }

    public ITextNode GetTextNodeAttribute(string attributeName)
    {
      return this.Attributes.FirstOrDefault(a => a.Name == attributeName);
    }

    public string GetAttributeValue(string attributeName, string defaultValue = "")
    {
      var value = this.GetTextNodeAttribute(attributeName)?.Value;
      return !string.IsNullOrEmpty(value) ? value : defaultValue;
    }

    bool ITextNode.SetValue(string value)
    {
      return false;
    }
  }
}
