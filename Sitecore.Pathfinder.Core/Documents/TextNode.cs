namespace Sitecore.Pathfinder.Documents
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class TextNode : ITextNode
  {
    public static readonly ITextNode Empty = new TextNode(Documents.DocumentSnapshot.Empty, TextPosition.Empty, string.Empty);

    public TextNode([NotNull] IDocumentSnapshot documentSnapshot)
    {
      this.DocumentSnapshot = documentSnapshot;
      this.Position = TextPosition.Empty;
      this.Name = string.Empty;
      this.Value = string.Empty;
      this.Parent = null;
    }

    public TextNode([NotNull] IDocumentSnapshot documentSnapshot, TextPosition position, [NotNull] string name, [NotNull] string value = "", [CanBeNull] ITextNode parent = null)
    {
      this.DocumentSnapshot = documentSnapshot;
      this.Name = name;
      this.Value = value;
      this.Parent = parent;
      this.Position = position;
    }

    public IList<ITextNode> Attributes { get; } = new List<ITextNode>();

    public IList<ITextNode> ChildNodes { get; } = new List<ITextNode>();

    public IDocumentSnapshot DocumentSnapshot { get; }

    public string Name { get; }

    public ITextNode Parent { get; }

    public TextPosition Position { get; }

    public string Value { get; }

    public ITextNode GetAttribute(string attributeName)
    {
      return this.Attributes.FirstOrDefault(a => a.Name == attributeName);
    }

    public string GetAttributeValue(string attributeName, string defaultValue = "")
    {
      var value = this.GetAttribute(attributeName)?.Value;
      return !string.IsNullOrEmpty(value) ? value : defaultValue;
    }

    public virtual void SetValue([NotNull] string value)
    {
      throw new InvalidOperationException("Cannot set name");
    }
  }
}
