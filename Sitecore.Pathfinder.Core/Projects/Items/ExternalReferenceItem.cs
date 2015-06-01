namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  public class ExternalReferenceItem : ItemBase
  {
    public ExternalReferenceItem([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ISnapshot snapshot) : base(project, projectUniqueId, snapshot)
    {
      this.IsEmittable = false;
    }
  }
}