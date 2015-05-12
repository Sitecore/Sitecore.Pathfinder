namespace Sitecore.Pathfinder.Emitters
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.IO;
  using Sitecore.Pathfinder.Configuration;
  using Sitecore.Pathfinder.Data;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;
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

      var project = this.ProjectService.LoadProject();

      this.Emit(project);
    }

    protected virtual void Emit([NotNull] IProject project)
    {
      // todo: use abstract factory pattern
      var context = new EmitContext(this.CompositionService, this.Trace, this.DataService, this.FileSystem).Load(project);

      var emitters = this.Emitters.OrderBy(e => e.Sortorder).ToList();

      var retries = new List<Tuple<ProjectItem, Exception>>();

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

    protected virtual void EmitProjectItem([NotNull] IEmitContext context, [NotNull] ProjectItem projectItem, [NotNull] List<IEmitter> emitters, [NotNull] ICollection<Tuple<ProjectItem, Exception>> retries)
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
          retries.Add(new Tuple<ProjectItem, Exception>(projectItem, ex));
        }
        catch (BuildException ex)
        {
          this.Trace.TraceError(ex.Text, ex.FileName, ex.Line, ex.Column, ex.Args);
        }
        catch (Exception ex)
        {
          retries.Add(new Tuple<ProjectItem, Exception>(projectItem, ex));
        }

        break;
      }
    }

    protected virtual void RetryEmit([NotNull] IEmitContext context, [NotNull] List<IEmitter> emitters, [NotNull] ICollection<Tuple<ProjectItem, Exception>> retries)
    {
      while (true)
      {
        var retryAgain = new List<Tuple<ProjectItem, Exception>>();
        foreach (var projectItem in retries.Reverse().Select(retry => retry.Item1))
        {
          try
          {
            this.EmitProjectItem(context, projectItem, emitters, retryAgain);
          }
          catch (Exception ex)
          {
            retries.Add(new Tuple<ProjectItem, Exception>(projectItem, ex));
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
          this.Trace.TraceError(buildException.Text, buildException.FileName, buildException.Line, buildException.Column, buildException.Args);
        }
        else if (exception != null)
        {
          this.Trace.TraceError(Texts.Text9998, projectItem.SourceFile.SourceFileName, 0, 0, exception.Message);
        }
        else
        {
          this.Trace.TraceError(Texts.Text9999, "An error occured in " + projectItem.SourceFile.SourceFileName);
        }
      }
    }
  }
}