namespace Sitecore.Pathfinder.Building
{
  using Sitecore.Pathfinder.Diagnostics;

  public interface ITask
  {
    [NotNull]
    string TaskName { get; }

    void Execute([NotNull] IBuildContext context);
  }
}
