namespace Sitecore.Pathfinder.Building.Initializing.Initialization
{
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;

  [Export(typeof(ITask))]
  public class Init : TaskBase
  {
    public Init() : base("init")
    {
    }

    public override void Execute([NotNull] IBuildContext context)
    {
      var projectDirectory = context.SolutionDirectory;
      if (!context.FileSystem.DirectoryExists(projectDirectory))
      {
        this.CreateProjectDirectory(context, projectDirectory);
        return;
      }

      var configFileName = Path.Combine(projectDirectory, context.Configuration.Get(Constants.ConfigFileName));
      if (!context.FileSystem.FileExists(configFileName))
      {
        this.CreateConfigurationFile(context, projectDirectory);
      }
    }

    protected virtual void CopyResourceFiles([NotNull] IBuildContext context, [NotNull] string projectDirectory)
    {
      var sourceDirectory = Path.Combine(context.Configuration.Get(Constants.ToolsPath), "wwwroot\\project\\*");

      context.FileSystem.XCopy(sourceDirectory, projectDirectory);
    }

    protected virtual void CreateConfigurationFile([NotNull] IBuildContext context, [NotNull] string projectDirectory)
    {
      this.CopyResourceFiles(context, projectDirectory);
      context.Trace.Writeline(ConsoleTexts.Text4002, context.Configuration.Get(Constants.ConfigFileName));
    }

    protected virtual void CreateProjectDirectory([NotNull] IBuildContext context, [NotNull] string projectDirectory)
    {
      context.FileSystem.CreateDirectory(projectDirectory);
      this.CopyResourceFiles(context, projectDirectory);

      context.Trace.Writeline(ConsoleTexts.Text4000);
      context.Trace.Writeline(ConsoleTexts.Text4001, context.Configuration.Get(Constants.ConfigFileName));
    }
  }
}
