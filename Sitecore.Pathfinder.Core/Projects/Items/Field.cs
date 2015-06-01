namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  // todo: consider basing this on ProjectElement
  public class Field
  {
    public Field([NotNull] string fieldName, [NotNull] string language, int version, [NotNull] ITextNode textNode, [NotNull] string valueHint = "")
    {
      this.FieldName = fieldName;
      this.Language = language;
      this.Version = version;
      this.ValueProperty = new Property(textNode);
      this.ValueHint = valueHint;
    }

    [NotNull]
    public string Language { get; }

    [NotNull]
    public string FieldName { get; }

    [NotNull]
    public Property ValueProperty { get; }

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

    public int Version { get; }
  }
}