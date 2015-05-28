namespace Sitecore.Pathfinder.Building.Initializing.InstallLayout
{
  using System.ComponentModel.Composition;
  using System.IO;

  [Export(typeof(ITask))]
  public class InstallLayout : TaskBase
  {
    public InstallLayout() : base("install-layout")
    {
    }

    public override void Run(IBuildContext context)
    {
      context.Trace.TraceInformation(Texts.Creating__layout__directory___);

      var sourceDirectory = Path.Combine(context.Configuration.Get(Constants.Configuration.ToolsDirectory), "templates\\layout\\*");
      var destinationDirectory = Path.Combine(Path.Combine(context.SolutionDirectory, context.Project.Options.ProjectDirectory), "layout");

      context.FileSystem.XCopy(sourceDirectory, destinationDirectory);
    }
  }
}
