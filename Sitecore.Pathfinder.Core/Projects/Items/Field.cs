namespace Sitecore.Pathfinder.Projects.Items
{
  using System;
  using System.Diagnostics;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Templates;

  // todo: consider basing this on ProjectElement
  [DebuggerDisplay("{GetType().Name,nq}: {FieldName,nq} = {Value}")]
  public class Field
  {
    public Field([NotNull] Item item, [NotNull] string fieldName, [NotNull] string language, int version, [NotNull] string value, [NotNull] string valueHint = "")
    {
      this.Item = item;
      this.FieldName = new Attribute<string>("Name", fieldName);
      this.Language = new Attribute<string>("Language", language);
      this.Version = new Attribute<int>("Version", version);
      this.Value = new Attribute<string>("Value", value);
      this.ValueHint = valueHint;
    }

    [NotNull]
    public Attribute<string> FieldName { get; }

    [NotNull]
    public Item Item { get; set; }

    [NotNull]
    public Attribute<string> Language { get; }

    public TemplateField TemplateField => this.Item.Template.Sections.SelectMany(s => s.Fields).FirstOrDefault(f => string.Compare(f.Name, this.FieldName.Value, StringComparison.OrdinalIgnoreCase) == 0) ?? TemplateField.Empty;

    [NotNull]
    public Attribute<string> Value { get; }

    [NotNull]
    public string ValueHint { get; }

    [NotNull]
    public Attribute<int> Version { get; }
  }
}
