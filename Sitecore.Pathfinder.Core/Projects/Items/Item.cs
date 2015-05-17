namespace Sitecore.Pathfinder.Projects.Items
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TextDocuments;

  public enum MergingMatch
  {
    MatchUsingItemPath, 

    MatchUsingSourceFile
  }

  public class Item : ItemBase
  {
    public Item([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] IDocument document) : base(project, projectUniqueId, document)
    {
    }

    public Item([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ITextNode textNode) : base(project, projectUniqueId, textNode)
    {
    }

    [NotNull]
    public IList<Field> Fields { get; } = new List<Field>();

    public MergingMatch MergingMatch { get; set; }

    public bool OverwriteWhenMerging { get; set; }

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
        var field = this.Fields.FirstOrDefault(f => string.Compare(f.Name, newField.Name, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(f.Language, newField.Language, StringComparison.OrdinalIgnoreCase) == 0 && f.Version == newField.Version);
        if (field == null)
        {
          this.Fields.Add(newField);
          continue;
        }

        field.Value = newField.Value;
      }
    }
  }
}
