namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TextDocuments;

  // todo: consider basing this on ProjectElement
  public class Field
  {
    public Field([NotNull] ITextNode textNode, [CanBeNull] ITextNode valueTextNode)
    {
      this.TextNode = textNode;
      this.ValueTextNode = valueTextNode;
    }

    public Field([NotNull] ITextNode textNode, [NotNull] string name, [NotNull] string value)
    {
      this.TextNode = textNode;
      this.Name = name;
      this.Value = value;
      this.Language = string.Empty;
    }

    [NotNull]
    public string Language { get; set; } = string.Empty;

    [NotNull]
    public string Name { get; set; } = string.Empty;

    [NotNull]
    public ITextNode TextNode { get; }

    [NotNull]
    public string Value { get; set; } = string.Empty;

    [CanBeNull]
    public ITextNode ValueTextNode { get; }

    public int Version { get; set; }
  }
}
