namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  // todo: consider basing this on ProjectElement
  public class Field
  {
    public Field([NotNull] string fieldName, [NotNull] string language, int version, [NotNull] ITextNode nameTextNode, [NotNull] ITextNode valueTextNode, [NotNull] string valueHint = "")
    {
      this.FieldName = fieldName;
      this.Language = language;
      this.Version = version;
      this.NameProperty = new Property(nameTextNode);
      this.ValueProperty = new Property(valueTextNode);
      this.ValueHint = valueHint;
    }

    [NotNull]
    public string FieldName { get; }

    [NotNull]
    public string Language { get; }

    [NotNull]
    public Property NameProperty { get; }

    [NotNull]
    public string Value
    {
      get
      {
        return this.ValueProperty.Value;
      }

      set
      {
        this.ValueProperty.Value = value;
      }
    }

    [NotNull]
    public string ValueHint { get; }

    [NotNull]
    public Property ValueProperty { get; }

    public int Version { get; }
  }
}
