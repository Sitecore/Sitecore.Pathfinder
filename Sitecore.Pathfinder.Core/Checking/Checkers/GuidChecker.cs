namespace Sitecore.Pathfinder.Checking.Checkers
{
  using System.ComponentModel.Composition;

  [Export(typeof(IChecker))]
  public class GuidChecker : CheckerBase
  {
    public override void Check(ICheckerContext context)
    {
      foreach (var projectItem1 in context.Project.Items)
      {
        foreach (var projectItem2 in context.Project.Items)
        {
          if (projectItem1 == projectItem2)
          {
            continue;
          }

          if (projectItem1.Guid != projectItem2.Guid)
          {
            continue;
          }

          context.Trace.TraceError("Unique ID clash", projectItem1.QualifiedName);
          context.IsDeployable = false;
        }
      }
    }
  }
}
