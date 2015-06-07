namespace Sitecore.Pathfinder.Projects.Items
{
  using System;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.Templates;

  // todo: consider basing this on ProjectElement
  public class Field
  {
    public Field([NotNull] Item item, [NotNull] string fieldName, [NotNull] string language, int version, [NotNull] ITextNode nameTextNode, [NotNull] ITextNode valueTextNode, [NotNull] string valueHint = "")
    {
      this.Item = item;
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
    public Item Item { get; set; }

    [NotNull]
    public string Language { get; }

    [NotNull]
    public Property NameProperty { get; }

    public TemplateField TemplateField => this.Item.Template.Sections.SelectMany(s => s.Fields).FirstOrDefault(f => string.Compare(f.Name, this.FieldName, StringComparison.OrdinalIgnoreCase) == 0) ?? TemplateField.Empty;

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
