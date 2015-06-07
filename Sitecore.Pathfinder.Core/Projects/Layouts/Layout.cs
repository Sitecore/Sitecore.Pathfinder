namespace Sitecore.Pathfinder.Projects.Layouts
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.Files;

  public class Layout : ContentFile
  {
    public Layout([NotNull] IProject project, [NotNull] ISnapshot snapshot) : base(project, snapshot)
    {
    }
  }
}
