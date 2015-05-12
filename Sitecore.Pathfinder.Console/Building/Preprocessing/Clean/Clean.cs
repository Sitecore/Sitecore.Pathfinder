namespace Sitecore.Pathfinder.Building.Preprocessing.Clean
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;

  [Export(typeof(ITask))]
  public class Clean : TaskBase
  {
    public Clean() : base("clean")
    {
    }

    public override void Run(IBuildContext context)
    {
      context.Trace.TraceInformation(Texts.Text1001);

      foreach (var projectItem in context.Project.Items)
      {
        projectItem.TextSpan.IsModified = true;
      }
    }
  }
}