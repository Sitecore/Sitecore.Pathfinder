namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TextDocuments;

  public class ExternalReferenceItem : ItemBase
  {
    public ExternalReferenceItem([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ITextNode textNode) : base(project, projectUniqueId, textNode)
    {
      this.IsEmittable = false;
    }

    public override void Bind()
    {
    }
  }
}