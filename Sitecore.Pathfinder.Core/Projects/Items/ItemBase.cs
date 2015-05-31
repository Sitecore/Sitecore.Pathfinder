namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  public abstract class ItemBase : ProjectItem
  {

    protected ItemBase([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ISnapshot snapshot) : base(project, projectUniqueId, snapshot)
    {
      this.ItemTextNode = new TextNode(snapshot);
    }

    protected ItemBase([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ITextNode itemTextNode) : base(project, projectUniqueId, itemTextNode.Snapshot)
    {
      this.ItemTextNode = itemTextNode;
    }

    [NotNull]
    public string DatabaseName { get; set; } = string.Empty;

    [NotNull]
    public string Icon { get; set; } = string.Empty;

    public bool IsEmittable { get; set; } = true;

    [NotNull]
    public string ItemIdOrPath { get; set; } = string.Empty;

    [NotNull]
    public string ItemName { get; set; } = string.Empty;

    public override string QualifiedName => this.ItemIdOrPath;

    public override string ShortName => this.ItemName;

    // todo: move to Item
    [NotNull]
    public string TemplateIdOrPath { get; set; } = string.Empty;

    [NotNull]
    public ITextNode ItemTextNode { get; }
  }
}
