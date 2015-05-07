namespace Sitecore.Pathfinder.Models.Items
{
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class FieldModel
  {
    public FieldModel([NotNull] string sourceFileName)
    {
      this.SourceFileName = sourceFileName;
      this.Name = string.Empty;
      this.Language = string.Empty;
      this.Value = string.Empty;
    }

    [NotNull]
    public string Language { get; set; }

    [NotNull]
    public string Name { get; set; }

    [CanBeNull]
    public XElement SourceElement { get; set; }

    [NotNull]
    public string SourceFileName { get; }

    [NotNull]
    public string Value { get; set; }

    public int Version { get; set; }
  }
}
