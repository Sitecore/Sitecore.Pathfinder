namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.IO;

  public abstract class ItemBase : ProjectItem
  {
    protected ItemBase([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ITextNode textNode, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemIdOrPath) : base(project, projectUniqueId, textNode.Snapshot)
    {
      this.DatabaseName = databaseName;
      this.ItemIdOrPath = itemIdOrPath;

      this.ItemName = new Attribute<string>("ItemName", itemName);
    }

    [NotNull]
    public string DatabaseName { get; private set; }

    [NotNull]                                        
    public string Icon { get; set; } = string.Empty;

    public bool IsEmittable { get; set; } = true;

    [NotNull]
    public string ItemIdOrPath { get; private set; }

    [NotNull]
    public Attribute<string> ItemName { get; }

    public override string QualifiedName => this.ItemIdOrPath;

    public override string ShortName => this.ItemName.Value;

    public override void Rename(string newQualifiedName)
    {
      this.ItemIdOrPath = newQualifiedName;
      this.ItemName.SetValue(PathHelper.GetFileNameWithoutExtensions(newQualifiedName));

      if (this.ItemName.Source != null)
      {
        this.ItemName.Source.SetValue(this.ItemName.Value);
      }
    }

    protected override void Merge(IProjectItem newProjectItem, bool overwrite)
    {
      base.Merge(newProjectItem, overwrite);

      var newItemBase = newProjectItem as ItemBase;
      if (newItemBase == null)
      {
        return;
      }

      if (overwrite)
      {
        this.ItemName.SetValue(newItemBase.ItemName.Value, newItemBase.ItemName.Source);

        this.ItemIdOrPath = newItemBase.ItemIdOrPath;
        this.DatabaseName = newItemBase.DatabaseName;
        this.IsEmittable = this.IsEmittable && newItemBase.IsEmittable;
      }

      if (!string.IsNullOrEmpty(newItemBase.DatabaseName))
      {
        this.DatabaseName = newItemBase.DatabaseName;
      }

      if (!string.IsNullOrEmpty(newItemBase.Icon))
      {
        this.Icon = newItemBase.Icon;
      }

      if (!newItemBase.IsEmittable)
      {
        this.IsEmittable = false;
      }

      this.References.AddRange(newItemBase.References);
    }
  }
}
