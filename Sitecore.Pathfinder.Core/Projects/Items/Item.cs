namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;

  public class Item : ItemBase
  {
    public item.LocationItem([NotNull] IProject project, [NotNull] Location sourceFile) : base(project, sourceFile)
    {
    }
  }
}
