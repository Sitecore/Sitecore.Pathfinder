namespace Sitecore.Pathfinder.Documents
{
  using Sitecore.Pathfinder.Diagnostics;

  public class Property
  {
    public Property([NotNull] ITextNode textNode)
    {
      this.TextNode = textNode;
    }

    [NotNull]
    public string Name => this.TextNode.Name;

    [NotNull]
    public ITextNode TextNode { get; }

    [NotNull]
    public string Value
    {
      get
      {
        return this.TextNode.Value;
      }

      set
      {
        this.TextNode.SetValue(value);
      }
    }
  }
}