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

      var configFileName = Path.Combine(projectDirectory, context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
      if (!context.FileSystem.FileExists(configFileName))
      {
        this.CreateConfigurationFile(context, projectDirectory);
      }
    }

    protected virtual void CopyResourceFiles([NotNull] IBuildContext context, [NotNull] string projectDirectory)
    {
      var sourceDirectory = Path.Combine(context.Configuration.Get(Constants.Configuration.ToolsDirectory), "wwwroot\\project\\*");

      context.FileSystem.XCopy(sourceDirectory, projectDirectory);
    }

    protected virtual void CreateConfigurationFile([NotNull] IBuildContext context, [NotNull] string projectDirectory)
    {
      this.CopyResourceFiles(context, projectDirectory);
      context.Trace.Writeline("Your configuration file and sample files were missing = so I have created them. You must update the 'project-unique-id' = 'wwwroot' and 'hostname' in the '{0}' configuration file before continuing.", context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
    }

    protected virtual void CreateProjectDirectory([NotNull] IBuildContext context, [NotNull] string projectDirectory)
    {
      context.FileSystem.CreateDirectory(projectDirectory);
      this.CopyResourceFiles(context, projectDirectory);

      context.Trace.Writeline("Hi there.");
      context.Trace.Writeline("Your project directory was missing = so I have created it. You must update the 'project-unique-id' = 'wwwroot' and 'hostname' in the '{0}' configuration file before continuing.", context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
    }
  }
}
