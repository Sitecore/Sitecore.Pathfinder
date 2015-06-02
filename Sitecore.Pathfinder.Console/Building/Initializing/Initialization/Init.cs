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

    public override void Run(IBuildContext context)
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
      var sourceDirectory = Path.Combine(context.Configuration.Get(Constants.Configuration.ToolsDirectory), "files\\project\\*");

      context.FileSystem.XCopy(sourceDirectory, projectDirectory);
    }

    protected virtual void CreateConfigurationFile([NotNull] IBuildContext context, [NotNull] string projectDirectory)
    {
      this.CopyResourceFiles(context, projectDirectory);
      context.Trace.Writeline(Texts.Your_configuration_file_and_sample_files_were_missing__so_I_have_created_them__You_must_update_the__project_unique_id____wwwroot__and__hostname__in_the___0___configuration_file_before_continuing_, context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
    }

    protected virtual void CreateProjectDirectory([NotNull] IBuildContext context, [NotNull] string projectDirectory)
    {
      context.FileSystem.CreateDirectory(projectDirectory);
      this.CopyResourceFiles(context, projectDirectory);

      context.Trace.Writeline(Texts.Hi_there_);
      context.Trace.Writeline(Texts.Your_project_directory_was_missing__so_I_have_created_it__You_must_update_the__project_unique_id____wwwroot__and__hostname__in_the___0___configuration_file_before_continuing_, context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
    }
  }
}
