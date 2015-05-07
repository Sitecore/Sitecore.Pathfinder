namespace Sitecore.Pathfinder.Projects
{
  using Sitecore.Pathfinder.Diagnostics;

  public class LogicalItem : ItemBase
  {
    public LogicalItem([NotNull] ISourceFile sourceFile, [NotNull] ProjectElementBase owner) : base(sourceFile)
    {
      this.Owner = owner;
    }

    [NotNull]
    public ProjectElementBase Owner { get; }
  }
}