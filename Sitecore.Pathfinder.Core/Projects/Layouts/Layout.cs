namespace Sitecore.Pathfinder.Projects.Layouts
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.Files;
  using Sitecore.Pathfinder.Projects.Items;

  public class Layout : ContentFile
  {
    public Layout([NotNull] IProject project, [NotNull] IDocumentSnapshot documentSnapshot) : base(project, documentSnapshot)
    {
    }
  }
}