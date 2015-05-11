namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;

  public class Item : ItemBase
  {
    public Item([NotNull] IProject project, [NotNull] ISourceFile sourceFile) : base(project, sourceFile)
    {
    }
  }
}
