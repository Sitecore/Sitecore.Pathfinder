namespace Sitecore.Pathfinder.Building.Linting
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Models;

  [Export(typeof(ITask))]
  public class Lint : TaskBase
  {
    public Lint() : base("lint")
    {
    }

    public override void Execute(IBuildContext context)
    {
      var outputDirectory = context.OutputDirectory;
      if (!context.FileSystem.DirectoryExists(outputDirectory))
      {
        return;
      }

      context.Trace.TraceInformation(ConsoleTexts.Text1010);

      var project = new Project(context.CompositionService, context.FileSystem, outputDirectory);
      project.Parse();
    }
  }
}
