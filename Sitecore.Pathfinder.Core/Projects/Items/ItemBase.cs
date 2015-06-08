namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  public abstract class ItemBase : ProjectItem
  {
    protected ItemBase([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ITextNode textNode, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemIdOrPath) : base(project, projectUniqueId, textNode.Snapshot)
    {
      this.DatabaseName = databaseName;
      this.ItemIdOrPath = itemIdOrPath;

      this.ItemName = new Attribute<string>("ItemName", itemName);
    }

    // todo: make read only
    [NotNull]
    public string DatabaseName { get; set; }

    [NotNull]                                        
    public string Icon { get; set; } = string.Empty;

    public bool IsEmittable { get; set; } = true;

    // todo: make read only
    [NotNull]
    public string ItemIdOrPath { get; set; }

    [NotNull]
    public Attribute<string> ItemName { get; }

    public override string QualifiedName => this.ItemIdOrPath;

    public override string ShortName => this.ItemName.Value;
  }
}
