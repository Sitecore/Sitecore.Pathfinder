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

      var project = context.Project;

      this.TraceProjectMessages(context);

      context.Trace.TraceInformation(Texts.Linting_items, context.Project.Items.Count().ToString());

      var checkerService = context.CompositionService.Resolve<ICheckerService>();

      checkerService.CheckProject(context.Project);
    }

    private void TraceProjectMessages([NotNull] IBuildContext context)
    {
      foreach (var message in context.Project.Messages)
      {
        switch (message.MessageType)
        {
          case MessageType.Error:
            context.Trace.TraceError(message.Text, message.FileName, message.Position);
            break;
          case MessageType.Warning:
            context.Trace.TraceWarning(message.Text, message.FileName, message.Position);
            break;
          default:
            context.Trace.TraceInformation(message.Text, message.FileName, message.Position);
            break;
        }
      }
    }
  }
}
