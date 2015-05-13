namespace Sitecore.Pathfinder.Projects.Items
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.CompilerServices;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  public abstract class ItemBase : ProjectItem
  {
    protected ItemBase([NotNull] IProject project, [NotNull] ITreeNode treeNode) : base(project, treeNode)
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
          field = new Field(this.TreeNode)
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
    public string ItemName { get; set; }

    public override string QualifiedName => this.ItemIdOrPath;

    public override string ShortName => this.ItemName;

    [NotNull]
    public string TemplateIdOrPath { get; set; } = string.Empty;

    public override void Bind()
    {
      this.References.Clear();

      if (!string.IsNullOrEmpty(this.TemplateIdOrPath))
      {
        this.References.AddTemplateReference(this.TemplateIdOrPath);
      }

      foreach (var field in this.Fields)
      {
        if (field.Value.StartsWith("/sitecore", StringComparison.OrdinalIgnoreCase))
        {
          this.References.AddFieldReference(field.Value);
        }
      }

      this.IsBound = true;
    }
  }
}
