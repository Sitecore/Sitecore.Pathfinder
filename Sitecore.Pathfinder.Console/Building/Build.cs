namespace Sitecore.Pathfinder.Building
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.ConfigurationExtensions;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;

  [Export]
  public class Build
  {
    private static readonly char[] Space = 
    {
      ' '
    };

    [ImportingConstructor]
    public Build([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] ITraceService traceService, [NotNull] IFileSystemService fileSystemService, [NotNull] IProjectService projectService)
    {
      this.CompositionService = compositionService;
      this.Configuration = configuration;
      this.Trace = traceService;
      this.FileSystemService = fileSystemService;
      this.ProjectService = projectService;
    }

    [NotNull]
    [ImportMany]
    public IEnumerable<ITask> Tasks { get; [UsedImplicitly] private set; }

    [NotNull]
    protected ICompositionService CompositionService { get; }

    [NotNull]
    protected IConfiguration Configuration { get; }

    [NotNull]
    protected IFileSystemService FileSystemService { get; }

    [NotNull]
    protected IProjectService ProjectService { get; }

    [NotNull]
    protected ITraceService Trace { get; }

    public virtual void Start()
    {
      this.LoadConfiguration();

      var project = this.LoadProject();

      // todo: use abstract factory pattern
      var context = new BuildContext(this.Configuration, this.Trace, this.CompositionService, this.FileSystemService).Load(project);

      this.Execute(context);
    }

    protected virtual void Execute([NotNull] IBuildContext context)
    {
      var pipeline = this.GetPipeline(context);
      if (!pipeline.Any())
      {
        context.Trace.TraceWarning(ConsoleTexts.Text2000);
        return;
      }

      foreach (var taskName in pipeline)
      {
        this.ExecuteTask(context, taskName);

        if (context.IsAborted)
        {
          break;
        }
      }
    }

    protected virtual void ExecuteTask([NotNull] IBuildContext context, [NotNull] string taskName)
    {
      var task = this.Tasks.FirstOrDefault(t => string.Compare(t.TaskName, taskName, StringComparison.OrdinalIgnoreCase) == 0);
      if (task == null)
      {
        context.Trace.TraceError(ConsoleTexts.Text3001, taskName);
        return;
      }

      try
      {
        task.Execute(context);
      }
      catch (BuildException ex)
      {
        context.Trace.TraceError(ex.Text, ex.FileName, ex.Line, ex.Column, ex.Message);
        context.IsAborted = true;

        if (string.Compare(context.Configuration.Get(Constants.DebugMode), "true", StringComparison.OrdinalIgnoreCase) == 0)
        {
          throw;
        }
      }
      catch (Exception ex)
      {
        context.Trace.TraceError(ConsoleTexts.Text3009, ex.Message + ex.StackTrace);
        context.IsAborted = true;

        if (string.Compare(context.Configuration.Get(Constants.DebugMode), "true", StringComparison.OrdinalIgnoreCase) == 0)
        {
          throw;
        }
      }
    }

    [NotNull]
    protected virtual IEnumerable<string> GetPipeline([NotNull] IBuildContext context)
    {
      string pipeline;

      var run = context.Configuration.Get("run") ?? string.Empty;
      if (!string.IsNullOrEmpty(run))
      {
        pipeline = context.Configuration.Get(run + ":pipeline");

        if (string.IsNullOrEmpty(pipeline))
        {
          pipeline = run;
        }
      }
      else
      {
        pipeline = context.Configuration.Get("pipeline");
      }

      var taskNames = pipeline.Split(Space, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList();

      if (taskNames.Any())
      {
        // inject 'before-build' task as the first task
        taskNames.Insert(0, "before-build");
      }

      return taskNames;
    }

    protected virtual void LoadConfiguration()
    {
      var configuration = this.Configuration as IConfigurationSourceContainer;
      if (configuration == null)
      {
        throw new ConfigurationException(ConsoleTexts.Text3000);
      }

      // cut off executable name
      var commandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToArray();

      // add system config
      var fileName = Path.Combine(configuration.Get(Constants.ToolsPath), configuration.Get(Constants.ConfigFileName));
      if (!File.Exists(fileName))
      {
        throw new ConfigurationException(ConsoleTexts.Text3002, fileName);
      }

      configuration.AddJsonFile(fileName);

      // add command line
      configuration.AddCommandLine(commandLineArgs);

      // set website path
      var toolsDirectory = configuration.Get(Constants.ToolsPath);
      var solutionDirectory = PathHelper.Combine(toolsDirectory, configuration.Get(Constants.SolutionDirectory) ?? string.Empty);
      configuration.Set(Constants.SolutionDirectory, solutionDirectory);

      // add build config
      var websiteConfigFileName = PathHelper.Combine(solutionDirectory, configuration.Get(Constants.ConfigFileName));
      if (File.Exists(websiteConfigFileName))
      {
        configuration.AddFile(websiteConfigFileName);
      }

      // set project path
      var projectDirectory = PathHelper.NormalizeFilePath(configuration.Get(Constants.ProjectDirectory) ?? string.Empty).TrimStart('\\');
      configuration.Set(Constants.ProjectDirectory, projectDirectory);

      // set output path
      var outputDirectory = PathHelper.Combine(solutionDirectory, PathHelper.NormalizeFilePath(configuration.Get(Constants.OutputDirectory) ?? string.Empty).TrimStart('\\'));
      if (!string.IsNullOrEmpty(outputDirectory))
      {
        configuration.Set(Constants.OutputDirectory, outputDirectory);
      }
    }

    [NotNull]
    protected virtual IProject LoadProject()
    {
      this.Trace.TraceInformation(ConsoleTexts.Text1011);

      // todo: refactor this
      var projectDirectory = PathHelper.Combine(this.Configuration.Get(Constants.SolutionDirectory), this.Configuration.Get(Constants.ProjectDirectory));
      var databaseName = this.Configuration.Get(Constants.Database);
      var ignoreDirectories = this.Configuration.Get(Constants.SystemDirectories).Split(Space, StringSplitOptions.RemoveEmptyEntries).ToList();

      ignoreDirectories.Add(Path.GetFileName(this.Configuration.Get(Constants.ToolsPath)));
      ignoreDirectories.Add(Path.GetFileName(this.Configuration.Get(Constants.OutputDirectory)));

      return this.ProjectService.LoadProject(projectDirectory, databaseName, ignoreDirectories.ToArray());
    }
  }
}
