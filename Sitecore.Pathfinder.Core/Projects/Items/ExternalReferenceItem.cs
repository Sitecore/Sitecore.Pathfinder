namespace Sitecore.Pathfinder.Projects.Items
{
  using System.Diagnostics;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TextDocuments;

  public class ExternalReferenceItem : ItemBase
  {
    public ExternalReferenceItem([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] IDocumentSnapshot documentSnapshot) : base(project, projectUniqueId, documentSnapshot)
    {
      this.IsEmittable = false;
    }
  }
}