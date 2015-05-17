namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TextDocuments;

  public abstract class ItemBase : ProjectItem
  {
    protected ItemBase([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] IDocument document) : base(project, projectUniqueId, document)
    {
      this.TextNode = new TextNode(document);
    }

    protected ItemBase([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ITextNode textNode) : base(project, projectUniqueId, textNode.Document)
    {
      this.TextNode = textNode;
    }

    [NotNull]
    public string DatabaseName { get; set; } = string.Empty;

    [NotNull]
    public string Icon { get; set; } = string.Empty;

    public bool IsEmittable { get; set; } = true;

    [NotNull]
    public string ItemIdOrPath { get; set; } = string.Empty;

    [NotNull]
    public string ItemName { get; set; }

    public override string QualifiedName => this.ItemIdOrPath;

    public override string ShortName => this.ItemName;

    // todo: move to Item
    [NotNull]
    public string TemplateIdOrPath { get; set; } = string.Empty;

    [NotNull]
    public ITextNode TextNode { get; }

    public override void Bind()
    {
      this.References.Clear();

      if (!string.IsNullOrEmpty(this.TemplateIdOrPath))
      {
        this.References.AddTemplateReference(this.TemplateIdOrPath);
      }
    }
  }
}
