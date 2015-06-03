namespace Sitecore.Pathfinder.Emitters
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using NuGet;
  using Sitecore.Pathfinder.Configuration;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Extensions;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.SecurityModel;

  [Export]
  public class Emitter
  {
    [ImportingConstructor]
    public Emitter([NotNull] ICompositionService compositionService, [NotNull] IConfigurationService configurationService, [NotNull] ITraceService traceService, [NotNull] IProjectService projectService)
    {
      this.CompositionService = compositionService;
      this.ConfigurationService = configurationService;
      this.Trace = traceService;
      this.ProjectService = projectService;
    }

    [NotNull]
    protected ICompositionService CompositionService { get; }

    [NotNull]
    protected IConfigurationService ConfigurationService { get; set; }

    [NotNull]
    [ImportMany]
    protected IEnumerable<IEmitter> Emitters { get; private set; }

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

    protected virtual void BuildNupkgFile([NotNull] IEmitContext context, [NotNull] string nuspecFileName)
    {
      var nupkgFileName = Path.ChangeExtension(nuspecFileName, "nupkg");

      try
      {
        context.FileSystem.DeleteFile(nupkgFileName);

        var packageBuilder = new PackageBuilder(nuspecFileName, Path.GetDirectoryName(nupkgFileName), NullPropertyProvider.Instance, false);

        using (var nupkg = new FileStream(nupkgFileName, FileMode.Create))
        {
          packageBuilder.Save(nupkg);
        }
      }
      catch (Exception ex)
      {
        this.Trace.TraceError(ex.Message);
      }
    }

    protected virtual void Emit([NotNull] IProject project)
    {
      var context = this.CompositionService.Resolve<IEmitContext>().With(project);

      this.TraceProjectDiagnostics(context);

      var emitters = this.Emitters.OrderBy(e => e.Sortorder).ToList();
      var retries = new List<Tuple<IProjectItem, Exception>>();

      // todo: use proper user
      using (new SecurityDisabler())
      {
        this.Emit(context, project, emitters, retries);
        this.EmitRetry(context, emitters, retries);
      }
    }

    protected virtual void Emit([NotNull] IEmitContext context, [NotNull] IProject project, [NotNull] List<IEmitter> emitters, [NotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
    {
      foreach (var projectItem in project.Items)
      {
        this.EmitProjectItem(context, projectItem, emitters, retries);
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
        catch (RetryableEmitException ex)
        {
          retries.Add(new Tuple<IProjectItem, Exception>(projectItem, ex));
        }
        catch (EmitException ex)
        {
          this.Trace.TraceError(ex.Text, ex.FileName, ex.Position, ex.Details);
        }
        catch (Exception ex)
        {
          retries.Add(new Tuple<IProjectItem, Exception>(projectItem, ex));
        }
      }
    }

    protected virtual void EmitRetry([NotNull] IEmitContext context, [NotNull] List<IEmitter> emitters, [NotNull] ICollection<Tuple<IProjectItem, Exception>> retries)
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

        var buildException = exception as EmitException;
        if (buildException != null)
        {
          this.Trace.TraceError(buildException.Text, buildException.FileName, buildException.Position, buildException.Details);
        }
        else if (exception != null)
        {
          this.Trace.TraceError(exception.Message, projectItem.Snapshot.SourceFile.FileName, TextPosition.Empty);
        }
        else
        {
          this.Trace.TraceError(Texts.An_error_occured, projectItem.Snapshot.SourceFile.FileName, TextPosition.Empty);
        }
      }
    }

    protected virtual void TraceProjectDiagnostics([NotNull] IEmitContext context)
    {
      foreach (var diagnostic in context.Project.Diagnostics)
      {
        switch (diagnostic.Severity)
        {
          case Severity.Error:
            context.Trace.TraceError(diagnostic.Text, diagnostic.FileName, diagnostic.Position);
            break;
          case Severity.Warning:
            context.Trace.TraceWarning(diagnostic.Text, diagnostic.FileName, diagnostic.Position);
            break;
          default:
            context.Trace.TraceInformation(diagnostic.Text, diagnostic.FileName, diagnostic.Position);
            break;
        }
      }
    }
  }
}
