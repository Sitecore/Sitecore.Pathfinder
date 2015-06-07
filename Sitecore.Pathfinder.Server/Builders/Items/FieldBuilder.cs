namespace Sitecore.Pathfinder.Builders.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  public class FieldBuilder
  {
    public FieldBuilder([NotNull] string fieldName, [NotNull] string language, int version, [NotNull] string value)
    {
      this.FieldName = fieldName;
      this.Language = language;
      this.Version = version;
      this.Value = value;
    }

    [NotNull]
    public string FieldName { get; }

    [NotNull]
    public string Language { get; }

    [NotNull]
    public Property NameProperty { get; }

    [NotNull]
    public string Value { get; set; }

    public int Version { get; }
  }
}
