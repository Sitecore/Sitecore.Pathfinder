namespace Sitecore.Pathfinder.Projects.Items
{
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  // todo: consider basing this on ProjectElement
  public class Field
  {
    public Field([NotNull] ISourceFile sourceFile)
    {
      this.SourceFile = sourceFile;
      this.Name = string.Empty;
      this.Value = string.Empty;
      this.Language = string.Empty;
    }

    public Field([NotNull] ISourceFile sourceFile, [NotNull] string name, [NotNull] string value)
    {
      this.SourceFile = sourceFile;
      this.Name = name;
      this.Value = value;
      this.Language = string.Empty;
    }

    [NotNull]
    public string Language { get; set; }

    [NotNull]
    public string Name { get; set; }

    [CanBeNull]
    public XElement SourceElement { get; set; }

    [NotNull]
    public ISourceFile SourceFile { get; }

    [NotNull]
    public string Value { get; set; }

    public int Version { get; set; }
  }
}
