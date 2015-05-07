namespace Sitecore.Pathfinder.Emitters
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Data;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Models;
  using Sitecore.SecurityModel;

  [Export]
  public class Emitter
  {
    [ImportingConstructor]
    public Emitter([NotNull] ICompositionService compositionService, [NotNull] IDataService dataService, [NotNull] ITraceService traceService, [NotNull] IFileSystemService fileSystem)
    {
      this.CompositionService = compositionService;
      this.DataService = dataService;
      this.Trace = traceService;
      this.FileSystem = fileSystem;
    }

    [NotNull]
    public ICompositionService CompositionService { get; }

    [NotNull]
    public IDataService DataService { get; }

    [NotNull]
    [ImportMany]
    public IEnumerable<IEmitter> Emitters { get; private set; }

    [NotNull]
    public IFileSystemService FileSystem { get; }

    [NotNull]
    public ITraceService Trace { get; }

    public virtual void Start([NotNull] string projectDirectory)
    {
      // todo: change to abstract factory pattern
      this.Trace.ProjectDirectory = projectDirectory;

      var project = this.ParseProject(projectDirectory);

      this.Emit(project);
    }

    protected virtual void Emit([NotNull] IProject project)
    {
      var context = new EmitContext(this.CompositionService, this.Trace, this.DataService, this.FileSystem);
      var emitters = this.Emitters.OrderBy(e => e.Sortorder).ToList();

      var retries = new List<Tuple<ModelBase, Exception>>();

      // todo: use proper user
      using (new SecurityDisabler())
      {
        foreach (var model in project.Models)
        {
          this.EmitModel(context, model, emitters, retries);
        }

        this.RetryEmit(context, emitters, retries);
      }
    }

    protected virtual void EmitModel([NotNull] IEmitContext context, [NotNull] ModelBase model, [NotNull] List<IEmitter> emitters, [NotNull] List<Tuple<ModelBase, Exception>> retries)
    {
      foreach (var emitter in emitters)
      {
        if (!emitter.CanEmit(context, model))
        {
          continue;
        }

        try
        {
          emitter.Emit(context, model);
        }
        catch (RetryableBuildException ex)
        {
          retries.Add(new Tuple<ModelBase, Exception>(model, ex));
        }
        catch (BuildException ex)
        {
          this.Trace.TraceError(ex.Text, ex.FileName, ex.Line, ex.Column, ex.Args);
        }
        catch (Exception ex)
        {
          retries.Add(new Tuple<ModelBase, Exception>(model, ex));
        }
      }
    }

    [NotNull]
    protected virtual IProject ParseProject([NotNull] string projectDirectory)
    {
      var project = new Project(this.CompositionService, this.FileSystem, projectDirectory);

      project.Parse();

      return project;
    }

    protected virtual void RetryEmit([NotNull] IEmitContext context, [NotNull] List<IEmitter> emitters, [NotNull] List<Tuple<ModelBase, Exception>> retries)
    {
      var count = retries.Count;

      while (count > 0)
      {
        var list = new List<Tuple<ModelBase, Exception>>(retries);
        list.Reverse();

        retries.Clear();

        foreach (var tuple in list)
        {
          var model = tuple.Item1;

          try
          {
            this.EmitModel(context, model, emitters, retries);
          }
          catch (Exception ex)
          {
            retries.Add(new Tuple<ModelBase, Exception>(model, ex));
          }
        }

        if (retries.Count >= count)
        {
          // did not succeed to install any items
          break;
        }

        count = retries.Count;
      }

      foreach (var tuple in retries)
      {
        var fileName = tuple.Item1;
        var ex = tuple.Item2;

        var buildException = ex as BuildException;
        if (buildException != null)
        {
          this.Trace.TraceError(buildException.Text, buildException.FileName, buildException.Line, buildException.Column, buildException.Args);
        }
        else if (ex != null)
        {
          this.Trace.TraceError(Texts.Text9998, fileName, 0, 0, ex.Message);
        }
        else
        {
          this.Trace.TraceError(Texts.Text9999, "An error occured in " + fileName);
        }
      }
    }
  }
}
