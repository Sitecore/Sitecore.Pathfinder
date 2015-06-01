namespace Sitecore.Pathfinder.Projects.Items
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.Templates;

  public enum MergingMatch
  {
    MatchUsingItemPath, 

    MatchUsingSourceFile
  }

  public class Item : ItemBase
  {
    public static readonly Item Empty = new Item(Projects.Project.Empty, "{935B8D6C-D25A-48B8-8167-2C0443D77027}", Documents.TextNode.Empty);

    public Item([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ISnapshot snapshot) : base(project, projectUniqueId, snapshot)
    {
    }

    public Item([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ITextNode textNode) : base(project, projectUniqueId, textNode)
    {
    }

    [NotNull]
    public IList<Field> Fields { get; } = new List<Field>();

    public MergingMatch MergingMatch { get; set; }

    public bool OverwriteWhenMerging { get; set; }

    [NotNull]
    public Template Template => this.Project.Items.OfType<Template>().FirstOrDefault(i => string.Compare(i.QualifiedName, this.TemplateIdOrPath, StringComparison.OrdinalIgnoreCase) == 0) ?? Template.Empty;

    [NotNull]
    public string TemplateIdOrPath { get; set; } = string.Empty;

    public void Merge([NotNull] Item newItem)
    {
      if (this.OverwriteWhenMerging)
      {
        this.OverwriteProjectUniqueId(newItem.ProjectUniqueId);
        this.ItemName = newItem.ItemName;
        this.ItemIdOrPath = newItem.ItemIdOrPath;
        this.DatabaseName = newItem.DatabaseName;
        this.IsEmittable = this.IsEmittable && newItem.IsEmittable;
        this.OverwriteWhenMerging = newItem.OverwriteWhenMerging;
      }

      // todo: throw exception if item and newItem value differ
      if (!string.IsNullOrEmpty(newItem.DatabaseName))
      {
        this.DatabaseName = newItem.DatabaseName;
      }

      if (!string.IsNullOrEmpty(newItem.TemplateIdOrPath))
      {
        this.TemplateIdOrPath = newItem.TemplateIdOrPath;
      }

      if (!string.IsNullOrEmpty(newItem.Icon))
      {
        this.Icon = newItem.Icon;
      }

      if (!newItem.IsEmittable)
      {
        this.IsEmittable = false;
      }

      this.MergingMatch = this.MergingMatch == MergingMatch.MatchUsingSourceFile && newItem.MergingMatch == MergingMatch.MatchUsingSourceFile ? MergingMatch.MatchUsingSourceFile : MergingMatch.MatchUsingItemPath;

      // todo: add TextNode
      // todo: add SourceFile
      foreach (var newField in newItem.Fields)
      {
        var field = this.Fields.FirstOrDefault(f => string.Compare(f.FieldName, newField.FieldName, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(f.Language, newField.Language, StringComparison.OrdinalIgnoreCase) == 0 && f.Version == newField.Version);
        if (field == null)
        {
          this.Fields.Add(newField);
          continue;
        }

        field.Value = newField.Value;
      }

      this.References.AddRange(newItem.References);
    }
  }
}