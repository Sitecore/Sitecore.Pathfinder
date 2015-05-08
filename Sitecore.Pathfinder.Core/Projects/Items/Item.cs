namespace Sitecore.Pathfinder.Projects.Items
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.CompilerServices;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class Item : ProjectItem
  {
    public Item([NotNull] ISourceFile sourceFile) : base(sourceFile)
    {
    }

    [NotNull]
    public string DatabaseName { get; set; } = string.Empty;

    [NotNull]
    public IList<Field> Fields { get; } = new List<Field>();

    [NotNull]
    public string Icon { get; set; } = string.Empty;

    public bool IsEmittable { get; set; } = true;

    [IndexerName("Field")]
    public string this[string fieldName]
    {
      get
      {
        var field = this.Fields.FirstOrDefault(f => string.Compare(f.Name, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
        return field?.Value ?? string.Empty;
      }

      set
      {
        var field = this.Fields.FirstOrDefault(f => string.Compare(f.Name, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
        if (field == null)
        {
          field = new Field(this.SourceFile)
          {
            Name = fieldName
          };

          this.Fields.Add(field);
        }

        field.Value = value;
      }
    }

    [NotNull]
    public string ItemIdOrPath { get; set; } = string.Empty;

    [NotNull]
    public string ItemName { get; set; } = string.Empty;

    public override string QualifiedName => this.QualifiedName;

    public override string ShortName => this.ItemName;

    [CanBeNull]
    public XElement SourceElement { get; set; }

    [NotNull]
    public string TemplateIdOrPath { get; set; } = string.Empty;
  }
}
