namespace Sitecore.Pathfinder.Builders.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects;

  public class FieldBuilder
  {
    public FieldBuilder([NotNull] Attribute<string> fieldName, [NotNull] string language, int version, [NotNull] string value)
    {
      this.FieldName = fieldName;
      this.Language = language;
      this.Version = version;
      this.Value = value;
    }

    [NotNull]
    public Attribute<string> FieldName { get; }

    [NotNull]
    public string Language { get; }

    [NotNull]
    public string Value { get; set; }

    public int Version { get; }
  }
}
