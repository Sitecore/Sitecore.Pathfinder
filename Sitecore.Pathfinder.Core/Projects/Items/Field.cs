namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TreeNodes;

  // todo: consider basing this on ProjectElement
  public class Field
  {
    public Field([NotNull] ITextSpan textSpan)
    {
      this.TextSpan = textSpan;
      this.Name = string.Empty;
      this.Value = string.Empty;
      this.Language = string.Empty;
    }

    public Field([NotNull] ITextSpan textSpan, [NotNull] string name, [NotNull] string value)
    {
      this.TextSpan = textSpan;
      this.Name = name;
      this.Value = value;
      this.Language = string.Empty;
    }

    [NotNull]
    public string Language { get; set; }

    [NotNull]
    public string Name { get; set; }

    [NotNull]
    public ITextSpan TextSpan { get; }

    [NotNull]
    public string Value { get; set; }

    public int Version { get; set; }
  }
}