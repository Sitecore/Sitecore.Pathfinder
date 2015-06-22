namespace Sitecore.Pathfinder.Projects.Components
{
  using Sitecore.Pathfinder.Diagnostics;

  public class Component : ItemBase
  {
    public Component([NotNull] ISourceFile sourceFile, [NotNull] LogicalItem publicItem, [NotNull] LogicalItem privateItem) : base(sourceFile)
    {
      this.PublicItem = publicItem;
      this.PrivateItem = privateItem;
    }

    [NotNull]
    public LogicalItem PrivateItem { get; }

    [NotNull]
    public LogicalItem PublicItem { get; }
  }
}