namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TreeNodes;

  public class Item : ItemBase
  {
    public Item([NotNull] IProject project, [NotNull] ITextSpan textSpan) : base(project, textSpan)
    {
    }
  }
}
