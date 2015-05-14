namespace Sitecore.Pathfinder.TextDocuments
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class TextNode : ITextNode
  {
    public static readonly ITextNode Empty = new TextNode(TextDocuments.TextDocument.Empty, string.Empty);

    public TextNode([NotNull] ITextDocument textDocument, [NotNull] string name, string value = "", int lineNumber = 0, int linePosition = 0, [CanBeNull] ITextNode parent = null)
    {
      this.TextDocument = textDocument;
      this.Name = name;
      this.Value = value;
      this.Parent = parent;
      this.LineNumber = lineNumber;
      this.LinePosition = linePosition;
    }

    public string Name { get; }

    public IList<ITextNode> ChildNodes { get; } = new List<ITextNode>();

    public string Value { get; }

    public string GetAttributeValue(string attributeName, string defaultValue = "")
    {
      var value = this.Attributes.FirstOrDefault(a => a.Name == attributeName)?.Value;
      return !string.IsNullOrEmpty(value) ? value : defaultValue;
    }

    public IList<ITextNode> Attributes { get; } = new List<ITextNode>();

    public ITextNode Parent { get; }

    public ITextDocument TextDocument { get; }

    public int LineNumber { get; }

    public int LinePosition { get; }

    public virtual void SetValue(string value)
    {
      throw new InvalidOperationException("Cannot set name");
    }
  }
}