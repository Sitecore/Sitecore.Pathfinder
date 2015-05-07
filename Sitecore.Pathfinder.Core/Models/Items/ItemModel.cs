namespace Sitecore.Pathfinder.Models.Items
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.CompilerServices;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class ItemModel : ItemModelBase
  {
    public ItemModel([NotNull] string sourceFileName) : base(sourceFileName)
    {
      this.TemplateIdOrPath = string.Empty;
      this.Fields = new List<FieldModel>();
    }

    [NotNull]
    public IList<FieldModel> Fields { get; }

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
          field = new FieldModel(this.SourceFileName)
          {
            Name = fieldName
          };

          this.Fields.Add(field);
        }

        field.Value = value;
      }
    }

    [CanBeNull]
    public XElement SourceElement { get; set; }

    [NotNull]
    public string TemplateIdOrPath { get; set; }
  }
}
