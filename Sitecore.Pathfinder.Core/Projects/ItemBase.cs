namespace Sitecore.Pathfinder.Projects
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.CompilerServices;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Items;

  public abstract class ItemBase : ProjectElementBase
  {
    protected ItemBase([NotNull] ISourceFile sourceFile) : base(sourceFile)
    {
      this.Name = string.Empty;
      this.DatabaseName = string.Empty;
      this.ItemIdOrPath = string.Empty;
      this.Icon = string.Empty;
      this.TemplateIdOrPath = string.Empty;
      this.Fields = new List<FieldModel>();
    }        

    [NotNull]
    public string DatabaseName { get; set; }

    [NotNull]
    public IList<FieldModel> Fields { get; }

    [NotNull]
    public string Icon { get; set; }

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
          field = new FieldModel(this.SourceFile)
          {
            Name = fieldName
          };

          this.Fields.Add(field);
        }

        field.Value = value;
      }
    }

    [NotNull]
    public string ItemIdOrPath { get; set; }

    [NotNull]
    public string Name { get; set; }

    public override string QualifiedName => this.QualifiedName;

    public override string ShortName => this.Name;

    [CanBeNull]
    public XElement SourceElement { get; set; }

    [NotNull]
    public string TemplateIdOrPath { get; set; }
  }
}
