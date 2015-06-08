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
    public static readonly Item Empty = new Item(Projects.Project.Empty, "{935B8D6C-D25A-48B8-8167-2C0443D77027}", TextNode.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

    public Item([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ITextNode textNode, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemIdOrPath, [NotNull] string templateIdOrPath) : base(project, projectUniqueId, textNode, databaseName, itemName, itemIdOrPath)
    {
      this.TemplateIdOrPath = new Attribute<string>("Template", templateIdOrPath);
    }

    [NotNull]
    public IList<Field> Fields { get; } = new List<Field>();

    public MergingMatch MergingMatch { get; set; }

    public bool OverwriteWhenMerging { get; set; }

    [NotNull]
    public Template Template => this.Project.Items.OfType<Template>().FirstOrDefault(i => string.Compare(i.QualifiedName, this.TemplateIdOrPath.Value, StringComparison.OrdinalIgnoreCase) == 0) ?? Template.Empty;

    [NotNull]
    public Attribute<string> TemplateIdOrPath { get; }

    public void Merge([NotNull] Item newProjectItem)
    {
      this.Merge(newProjectItem, this.OverwriteWhenMerging);
    }

    protected override void Merge(IProjectItem newProjectItem, bool overwrite)
    {
      base.Merge(newProjectItem, overwrite);

      var newItem = newProjectItem as Item;
      if (newItem == null)
      {
        return;
      }

      if (!string.IsNullOrEmpty(newItem.TemplateIdOrPath.Value))
      {
        this.TemplateIdOrPath.SetValue(newItem.TemplateIdOrPath.Value, newItem.TemplateIdOrPath.Source);
      }

      this.OverwriteWhenMerging = this.OverwriteWhenMerging && newItem.OverwriteWhenMerging;
      this.MergingMatch = this.MergingMatch == MergingMatch.MatchUsingSourceFile && newItem.MergingMatch == MergingMatch.MatchUsingSourceFile ? MergingMatch.MatchUsingSourceFile : MergingMatch.MatchUsingItemPath;

      // todo: add SourceFile
      foreach (var newField in newItem.Fields)
      {
        var field = this.Fields.FirstOrDefault(f => string.Compare(f.FieldName.Value, newField.FieldName.Value, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(f.Language.Value, newField.Language.Value, StringComparison.OrdinalIgnoreCase) == 0 && f.Version.Value == newField.Version.Value);
        if (field == null)
        {
          newField.Item = this;
          this.Fields.Add(newField);
          continue;
        }

        // todo: trace that field is being overwritten
        field.Value.SetValue(newField.Value.Value, newField.Value.Source);
      }
    }
  }
}
