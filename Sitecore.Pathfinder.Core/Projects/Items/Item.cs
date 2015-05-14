namespace Sitecore.Pathfinder.Projects.Items
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.CompilerServices;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TextDocuments;

  public class Item : ItemBase
  {
    public Item([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ITextNode textNode) : base(project, projectUniqueId, textNode)
    {
    }

    [NotNull]
    public IList<Field> Fields { get; } = new List<Field>();

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
          field = new Field(this.TextNode)
          {
            Name = fieldName
          };

          this.Fields.Add(field);
        }

        field.Value = value;
      }
    }

    public override void Bind()
    {
      base.Bind();

      foreach (var field in this.Fields)
      {
        // todo: use regular expression to detect paths, guids, piped guids - possibly an field handler for Link, Images, Rich Text fields
        if (field.Value.StartsWith("/sitecore", StringComparison.OrdinalIgnoreCase))
        {
          this.References.AddFieldReference(field.Value);
        }
      }
    }
  }
}