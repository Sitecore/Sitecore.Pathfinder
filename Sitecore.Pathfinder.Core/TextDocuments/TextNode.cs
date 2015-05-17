namespace Sitecore.Pathfinder.TextDocuments
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class TextNode : ITextNode
  {
    public static readonly ITextNode Empty = new TextNode(null, string.Empty);

    public TextNode([NotNull] IDocument document)
    {
      this.Document = document;
      this.Name = string.Empty;
      this.Value = string.Empty;
      this.Parent = null;
      this.LineNumber = 0;
      this.LinePosition = 0;
    }

    public TextNode([NotNull] IDocument document, [NotNull] string name, [NotNull] string value = "", int lineNumber = 0, int linePosition = 0, [CanBeNull] ITextNode parent = null)
    {
      this.Document = document;
      this.Name = name;
      this.Value = value;
      this.Parent = parent;
      this.LineNumber = lineNumber;
      this.LinePosition = linePosition;
    }

    public IList<ITextNode> Attributes { get; } = new List<ITextNode>();

    public IList<ITextNode> ChildNodes { get; } = new List<ITextNode>();

    public IDocument Document { get; }

    public int LineNumber { get; }

    public int LinePosition { get; }

    public string Name { get; }

    public ITextNode Parent { get; }

    public string Value { get; }

    public string GetAttributeValue(string attributeName, string defaultValue = "")
    {
      var value = this.Attributes.FirstOrDefault(a => a.Name == attributeName)?.Value;
      return !string.IsNullOrEmpty(value) ? value : defaultValue;
    }

    public virtual void SetValue([NotNull] string value)
    {
      throw new InvalidOperationException("Cannot set name");
    }
  }
}
