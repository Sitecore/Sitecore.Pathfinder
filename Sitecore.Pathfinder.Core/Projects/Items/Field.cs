namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  // todo: consider basing this on ProjectElement
  public class Field
  {
    public Field([NotNull] string fieldName, [NotNull] Property valueProperty)
    {
      this.FieldName = fieldName;
      this.ValueProperty = valueProperty;
    }

    [NotNull]
    public string Language { get; set; } = string.Empty;

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
    public string ValueHint { get; set; } = string.Empty;

    public int Version { get; set; }
  }
}