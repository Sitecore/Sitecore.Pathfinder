namespace Sitecore.Pathfinder.Emitters
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Configuration;
  using Sitecore.Pathfinder.Data;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.TextDocuments;
  using Sitecore.SecurityModel;

  [Export]
  public class Emitter
  {
    [ImportingConstructor]
    public Emitter([NotNull] ICompositionService compositionService, [NotNull] IConfigurationService configurationService, [NotNull] IDataService dataService, [NotNull] ITraceService traceService, [NotNull] IFileSystemService fileSystem, [Sitecore.NotNull] IProjectService projectService)
    {
      this.CompositionService = compositionService;
      this.ConfigurationService = configurationService;
      this.DataService = dataService;
      this.Trace = traceService;
      this.FileSystem = fileSystem;
      this.ProjectService = projectService;
    }

    [NotNull]
    protected ICompositionService CompositionService { get; }

    [NotNull]
    protected IConfigurationService ConfigurationService { get; set; }

    [NotNull]
    protected IDataService DataService { get; }

    [NotNull]
    [ImportMany]
    protected IEnumerable<IEmitter> Emitters { get; private set; }

    [NotNull]
    protected IFileSystemService FileSystem { get; }

    [NotNull]
    protected IProjectService ProjectService { get; }

    [NotNull]
    protected ITraceService Trace { get; }

    public virtual void Start()
    {
      this.ConfigurationService.Load(LoadConfigurationOptions.None);

      var project = this.ProjectService.LoadProjectFromConfiguration();

      this.Emit(project);
    }

    protected virtual void Emit([NotNull] IProject project)
    {
      // todo: use abstract factory pattern
      var context = new EmitContext(this.CompositionService, this.Trace, this.DataService, this.FileSystem).With(project);

      var emitters = this.Emitters.OrderBy(e => e.Sortorder).ToList();

      var retries = new List<Tuple<IProjectItem, Exception>>();

      // todo: use proper user
      using (new SecurityDisabler())
      {
        foreach (var projectItem in project.Items)
        {
          this.EmitProjectItem(context, projectItem, emitters, retries);
        }

        this.RetryEmit(context, emitters, retries);
      }
    }

    protected virtual void EmitProjectItem([NotNull] IEmitContext context, [NotNull] IProjectItem projectItem, [NotNull] List<IEmitter> emitters, [NotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
    {
      foreach (var emitter in emitters)
      {
        if (!emitter.CanEmit(context, projectItem))
        {
          continue;
        }

        try
        {
          emitter.Emit(context, projectItem);
        }
        catch (RetryableBuildException ex)
        {
          retries.Add(new Tuple<IProjectItem, Exception>(projectItem, ex));
        }
        catch (BuildException ex)
        {
          this.Trace.TraceError(ex.Text, ex.FileName, ex.Position, ex.Details);
        }
        catch (Exception ex)
        {
          retries.Add(new Tuple<IProjectItem, Exception>(projectItem, ex));
        }
      }
    }

    protected virtual void RetryEmit([NotNull] IEmitContext context, [NotNull] List<IEmitter> emitters, [NotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
    {
      while (true)
      {
        var retryAgain = new List<Tuple<IProjectItem, Exception>>();
        foreach (var projectItem in retries.Reverse().Select(retry => retry.Item1))
        {
          try
          {
            this.EmitProjectItem(context, projectItem, emitters, retryAgain);
          }
          catch (Exception ex)
          {
            retries.Add(new Tuple<IProjectItem, Exception>(projectItem, ex));
          }
        }

        if (retryAgain.Count >= retries.Count)
        {
          // did not succeed to install any items
          retries = retryAgain;
          break;
        }

        retries = retryAgain;
      }

      foreach (var retry in retries)
      {
        var projectItem = retry.Item1;
        var exception = retry.Item2;

        var buildException = exception as BuildException;
        if (buildException != null)
        {
          this.Trace.TraceError(buildException.Text, buildException.FileName, buildException.Position, buildException.Details);
        }
        else if (exception != null)
        {
          this.Trace.TraceError(exception.Message, projectItem.Document.SourceFile.FileName, TextPosition.Empty);
        }
        else
        {
          this.Trace.TraceError("An error occured", projectItem.Document.SourceFile.FileName, TextPosition.Empty);
        }
      }
    }
  }
}