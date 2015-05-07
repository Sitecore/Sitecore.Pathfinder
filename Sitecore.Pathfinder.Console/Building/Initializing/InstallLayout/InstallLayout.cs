namespace Sitecore.Pathfinder.Building.Initializing.InstallLayout
{
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;

  [Export(typeof(ITask))]
  public class InstallLayout : TaskBase
  {
    public InstallLayout() : base("install-layout")
    {
    }

    public override void Execute(IBuildContext context)
    {
      context.Trace.TraceInformation(ConsoleTexts.Text1006);

      var sourceDirectory = Path.Combine(context.Configuration.Get(Constants.ToolsPath), "templates\\layout\\*");
      var destinationDirectory = Path.Combine(Path.Combine(context.SolutionDirectory, context.ProjectDirectory), "layout");

      context.FileSystem.XCopy(sourceDirectory, destinationDirectory);
    }
  }
}
