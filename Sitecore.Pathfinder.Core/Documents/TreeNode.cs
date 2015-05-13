namespace Sitecore.Pathfinder.Documents
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class TreeNode : ITreeNode
  {
    public static readonly ITreeNode Empty = new TreeNode(Documents.Document.Empty, string.Empty);

    public TreeNode([NotNull] IDocument document, [NotNull] string name, string value = "", int lineNumber = 0, int linePosition = 0, [CanBeNull] ITreeNode parent = null)
    {
      this.Document = document;
      this.Name = name;
      this.Value = value;
      this.Parent = parent;
      this.LineNumber = lineNumber;
      this.LinePosition = linePosition;
    }

    public string Name { get; }

    public IList<ITreeNode> TreeNodes { get; } = new List<ITreeNode>();

    public string Value { get; }

    public string GetAttributeValue(string attributeName, string defaultValue = "")
    {
      var value = this.Attributes.FirstOrDefault(a => a.Name == attributeName)?.Value;
      return !string.IsNullOrEmpty(value) ? value : defaultValue;
    }

    public IList<ITreeNode> Attributes { get; } = new List<ITreeNode>();

    public ITreeNode Parent { get; }

    public IDocument Document { get; }

    public int LineNumber { get; }

    public int LinePosition { get; }

    public virtual void SetValue(string value)
    {
      throw new InvalidOperationException("Cannot set name");
    }
  }
}