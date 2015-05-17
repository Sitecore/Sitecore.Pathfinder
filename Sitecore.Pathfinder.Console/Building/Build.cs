namespace Sitecore.Pathfinder.Building
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Configuration;
  using Sitecore.Pathfinder.Diagnostics;
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
    public Build([NotNull] ICompositionService compositionService, [NotNull] IConfigurationService configurationService, [NotNull] ITraceService trace, [NotNull] IFileSystemService fileSystem, [NotNull] IProjectService projectService)
    {
      this.CompositionService = compositionService;
      this.ConfigurationService = configurationService;
      this.Trace = trace;
      this.FileSystem = fileSystem;
      this.ProjectService = projectService;
    }

    [NotNull]
    [ImportMany]
    public IEnumerable<ITask> Tasks { get; [UsedImplicitly] private set; }

    [NotNull]
    protected ICompositionService CompositionService { get; }

    [NotNull]
    protected IConfigurationService ConfigurationService { get; }

    [NotNull]
    protected IFileSystemService FileSystem { get; }

    [NotNull]
    protected IProjectService ProjectService { get; }

    [NotNull]
    protected ITraceService Trace { get; }

    public virtual void Start()
    {
      this.ConfigurationService.Load(LoadConfigurationOptions.IncludeCommandLine);

      this.Trace.TraceInformation(Texts.Text1011);
      var project = this.ProjectService.LoadProjectFromConfiguration();
      this.Trace.TraceInformation(Texts.Text1023, project.SourceFiles.Count, project.Items.Count());


      // todo: use abstract factory pattern
      var context = new BuildContext(this.ConfigurationService.Configuration, this.Trace, this.CompositionService, this.FileSystem).With(project);

      this.Run(context);
    }

    protected virtual void Run([NotNull] IBuildContext context)
    {
      var pipeline = this.GetPipeline(context);
      if (!pipeline.Any())
      {
        context.Trace.TraceWarning(Texts.Text1012);
        return;
      }

      foreach (var taskName in pipeline)
      {
        this.RunTask(context, taskName);

        if (context.IsAborted)
        {
          break;
        }
      }
    }

    protected virtual void RunTask([NotNull] IBuildContext context, [NotNull] string taskName)
    {
      var task = this.Tasks.FirstOrDefault(t => string.Compare(t.TaskName, taskName, StringComparison.OrdinalIgnoreCase) == 0);
      if (task == null)
      {
        context.Trace.TraceError(Texts.Text3001, taskName);
        return;
      }

      try
      {
        task.Run(context);
      }
      catch (BuildException ex)
      {
        context.Trace.TraceError(ex.Text, ex.FileName, ex.LineNumber, ex.LinePosition, ex.Message);
        context.IsAborted = true;

        if (string.Compare(context.Configuration.Get(Constants.Debug), "true", StringComparison.OrdinalIgnoreCase) == 0)
        {
          throw;
        }
      }
      catch (Exception ex)
      {
        context.Trace.TraceError(Texts.Text3009, ex.Message);
        context.IsAborted = true;

        if (string.Compare(context.Configuration.Get(Constants.Debug), "true", StringComparison.OrdinalIgnoreCase) == 0)
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
  }
}
