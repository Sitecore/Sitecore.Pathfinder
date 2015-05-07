namespace Sitecore.Pathfinder.Building
{
  using Sitecore.Pathfinder.Diagnostics;

  public abstract class TaskBase : ITask
  {
    protected TaskBase([NotNull] string taskName)
    {
      this.TaskName = taskName;
    }

    public string TaskName { get; }

    public abstract void Execute(IBuildContext context);
  }
}
