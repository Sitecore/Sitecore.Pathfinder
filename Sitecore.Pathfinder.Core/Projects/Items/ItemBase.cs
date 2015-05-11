namespace Sitecore.Pathfinder.Projects.Items
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public abstract class ItemBase : ProjectItem
  {
    protected ItemBase([NotNull] IProject project, [NotNull] ISourceFile sourceFile) : base(project, sourceFile)
    {
    }

    [NotNull]
    public string DatabaseName { get; set; } = string.Empty;

    [NotNull]
    public IList<Field> Fields { get; } = new List<Field>();

    [NotNull]
    public string Icon { get; set; } = string.Empty;

    public bool IsEmittable { get; set; } = true;

    [NotNull]
    public string ItemIdOrPath { get; set; } = string.Empty;

    [NotNull]
    public string ItemName { get; set; } = string.Empty;

    public override string QualifiedName => this.ItemIdOrPath;

    public override string ShortName => this.ItemName;

    [NotNull]
    public string TemplateIdOrPath { get; set; } = string.Empty;

    public override void Analyze()
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
          this.References.AddFieldReference(field);
        }
      }

      this.IsAnalyzed = true;
    }
  }
}
