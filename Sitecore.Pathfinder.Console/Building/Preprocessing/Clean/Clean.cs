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

    public override void Execute(IBuildContext context)
    {
      var outputDirectory = context.OutputDirectory;
      if (!context.FileSystem.DirectoryExists(outputDirectory))
      {
        return;
      }

      context.Trace.TraceInformation(ConsoleTexts.Text1001);

      foreach (var fileName in context.FileSystem.GetFiles(outputDirectory))
      {
        context.FileSystem.DeleteFile(fileName);
      }

      foreach (var dir in context.FileSystem.GetDirectories(outputDirectory))
      {
        context.FileSystem.DeleteDirectory(dir);
      }
    }
  }
}
