namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TextDocuments;

  // todo: consider basing this on ProjectElement
  public class Field
  {
    public Field([NotNull] ITextNode textNode)
    {
      this.TextNode = textNode;
      this.Name = string.Empty;
      this.Value = string.Empty;
      this.Language = string.Empty;
    }

    public Field([NotNull] ITextNode textNode, [NotNull] string name, [NotNull] string value)
    {
      this.TextNode = textNode;
      this.Name = name;
      this.Value = value;
      this.Language = string.Empty;
    }

    [NotNull]
    public string Language { get; set; }

    [NotNull]
    public string Name { get; set; }

    [NotNull]
    public ITextNode TextNode { get; }

    [NotNull]
    public string Value { get; set; }

    public int Version { get; set; }
  }
}