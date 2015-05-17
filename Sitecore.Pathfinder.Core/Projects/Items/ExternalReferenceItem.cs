namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TextDocuments;

  public class ExternalReferenceItem : ItemBase
  {
    public ExternalReferenceItem([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ITextNode document) : base(project, projectUniqueId, document)
    {
      this.IsEmittable = false;
    }

    public override void Bind()
    {
    }
  }
}