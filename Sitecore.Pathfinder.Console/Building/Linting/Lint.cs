namespace Sitecore.Pathfinder.Building.Linting
{
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Checking;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.CompositionServiceExtensions;

  [Export(typeof(ITask))]
  public class Lint : TaskBase
  {
    public Lint() : base("lint")
    {
    }

    public override void Run(IBuildContext context)
    {
      context.Trace.TraceInformation(Texts.Checking___);

      this.TraceDiagnostics(context);
    }

    protected void TraceDiagnostics([NotNull] IBuildContext context)
    {
      foreach (var diagnostic in context.Project.Diagnostics)
      {
        switch (diagnostic.Severity)
        {
          case Severity.Error:
            context.Trace.TraceError(diagnostic.Text, diagnostic.FileName, diagnostic.Position);
            break;
          case Severity.Warning:
            context.Trace.TraceWarning(diagnostic.Text, diagnostic.FileName, diagnostic.Position);
            break;
          default:
            context.Trace.TraceInformation(diagnostic.Text, diagnostic.FileName, diagnostic.Position);
            break;
        }
      }
    }
  }
}
