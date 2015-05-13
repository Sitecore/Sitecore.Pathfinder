namespace Sitecore.Pathfinder.Building.Linting
{
  using System.ComponentModel.Composition;
  using System.Linq;

  [Export(typeof(ITask))]
  public class Lint : TaskBase
  {
    public Lint() : base("lint")
    {
    }

    public override void Run(IBuildContext context)
    {
      context.Trace.TraceInformation(Texts.Text1010);
      context.Trace.TraceInformation(Texts.Text1021, context.Project.Items.Count);

      foreach (var projectItem1 in context.Project.Items)
      {
        foreach (var projectItem2 in context.Project.Items)
        {
          if (projectItem1 == projectItem2)
          {
            continue;
          }

          if (projectItem1.Guid == projectItem2.Guid)
          {
            context.Trace.TraceError(Texts.Text3029, projectItem1.QualifiedName, projectItem2.QualifiedName);
            context.IsDeployable = false;
          }
        }
      }

      foreach (var projectItem in context.Project.Items)
      {
        projectItem.Lint();
      }
    }
  }
}
