namespace Sitecore.Pathfinder.Building
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Configuration;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.CompositionServiceExtensions;
  using Sitecore.Pathfinder.Extensions.ConfigurationExtensions;

  [Export]
  public class Build
  {
    [ImportingConstructor]
    public Build([NotNull] ICompositionService compositionService, [NotNull] IConfigurationService configurationService)
    {
      this.CompositionService = compositionService;
      this.ConfigurationService = configurationService;
    }

    [NotNull]
    [ImportMany]
    public IEnumerable<ITask> Tasks { get; [UsedImplicitly] private set; }

    [NotNull]
    protected ICompositionService CompositionService { get; }

    [NotNull]
    protected IConfigurationService ConfigurationService { get; }

    public virtual void Start()
    {
      this.ConfigurationService.Load(LoadConfigurationOptions.IncludeCommandLine);

      var context = this.CompositionService.Resolve<IBuildContext>();
      this.Run(context);
    }

    [NotNull]
    protected virtual IEnumerable<string> GetPipeline([NotNull] IBuildContext context)
    {
      string pipeline;

      var run = context.Configuration.GetString("run");
      if (!string.IsNullOrEmpty(run))
      {
        pipeline = context.Configuration.GetString(run + ":pipeline");

        if (string.IsNullOrEmpty(pipeline))
        {
          pipeline = run;
        }
      }
      else
      {
        pipeline = context.Configuration.GetString("pipeline");
      }

      var taskNames = pipeline.Split(Constants.Space, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList();

      if (taskNames.Any())
      {
        // inject 'before-build' task as the first task
        taskNames.Insert(0, "before-build");
      }

      return taskNames;
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

        if (context.Configuration.GetBool(Constants.Configuration.Debug))
        {
          throw;
        }
      }
      catch (Exception ex)
      {
        context.Trace.TraceError(Texts.Text3009, ex.Message);
        context.IsAborted = true;

        if (context.Configuration.GetBool(Constants.Configuration.Debug))
        {
          throw;
        }
      }
    }
  }
}
