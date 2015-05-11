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

    public override void Run([NotNull] IBuildContext context)
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
      var sourceDirectory = Path.Combine(context.Configuration.Get(Constants.ToolsDirectory), "wwwroot\\project\\*");

      context.FileSystem.XCopy(sourceDirectory, projectDirectory);
    }

    protected virtual void CreateConfigurationFile([NotNull] IBuildContext context, [NotNull] string projectDirectory)
    {
      this.CopyResourceFiles(context, projectDirectory);
      context.Trace.Writeline(Texts.Text1015, context.Configuration.Get(Constants.ConfigFileName));
    }

    protected virtual void CreateProjectDirectory([NotNull] IBuildContext context, [NotNull] string projectDirectory)
    {
      context.FileSystem.CreateDirectory(projectDirectory);
      this.CopyResourceFiles(context, projectDirectory);

      context.Trace.Writeline(Texts.Text1013);
      context.Trace.Writeline(Texts.Text1014, context.Configuration.Get(Constants.ConfigFileName));
    }
  }
}
