namespace Sitecore.Pathfinder.Emitters
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Xml;
  using NuGet;
  using Sitecore.Data.Serialization;
  using Sitecore.Pathfinder.Configuration;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Extensions.CompositionServiceExtensions;
  using Sitecore.Pathfinder.Extensions.ConfigurationExtensions;
  using Sitecore.Pathfinder.IO;
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

      Manager.DumpTree();
    }

    protected virtual void BuildNuspecFile([NotNull] string nuspecFileName)
    {
      using (var output = new XmlTextWriter(nuspecFileName, Encoding.UTF8))
      {
        output.Formatting = Formatting.Indented;

        output.WriteStartElement("package");

        // todo: provide better package info
        output.WriteStartElement("metadata");
        output.WriteElementString("id", "Uninstall");
        output.WriteElementString("version", "1.0.0");
        output.WriteElementString("title", "Uninstall");
        output.WriteElementString("authors", "Sitecore Pathfinder");
        output.WriteElementString("owners", "Sitecore Pathfinder");
        output.WriteElementString("description", "Uninstall Package");
        output.WriteElementString("summary", "Uninstall Package");
        output.WriteElementString("tags", string.Empty);

        output.WriteEndElement();

        output.WriteStartElement("files");

        output.WriteStartElement("file");

        output.WriteAttributeString("src", "**/*");
        output.WriteAttributeString("target", string.Empty);

        output.WriteEndElement();

        output.WriteEndElement();

        output.WriteEndElement();
      }
    }

    protected virtual void Emit([NotNull] IProject project)
    {
      var context = this.CompositionService.Resolve<IEmitContext>().With(project);

      this.TraceProjectDiagnostics(context);

      if (context.FileSystem.DirectoryExists(context.UninstallDirectory))
      {
        context.FileSystem.DeleteDirectory(context.UninstallDirectory);
      }

      var emitters = this.Emitters.OrderBy(e => e.Sortorder).ToList();
      var retries = new List<Tuple<IProjectItem, Exception>>();

      // todo: use proper user
      using (new SecurityDisabler())
      {
        this.Emit(context, project, emitters, retries);
        this.EmitRetry(context, emitters, retries);
      }

      this.EmitUninstallPackage(context);
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

    protected virtual void EmitUninstallPackage([NotNull] IEmitContext context)
    {
      var systemConfigFileName = Path.Combine(context.Configuration.GetString(Pathfinder.Constants.Configuration.ToolsDirectory), context.Configuration.GetString(Pathfinder.Constants.Configuration.SystemConfigFileName));
      var projectConfigFileName = PathHelper.Combine(context.Project.Options.ProjectDirectory, context.Configuration.Get(Pathfinder.Constants.Configuration.ProjectConfigFileName));

      context.FileSystem.CreateDirectory(Path.Combine(context.UninstallDirectory, "content\\.sitecore.tools"));
      context.FileSystem.Copy(systemConfigFileName, Path.Combine(context.UninstallDirectory, "content\\.sitecore.tools\\" + context.Configuration.GetString(Pathfinder.Constants.Configuration.SystemConfigFileName)));
      context.FileSystem.Copy(projectConfigFileName, Path.Combine(context.UninstallDirectory, "content\\" + context.Configuration.Get(Pathfinder.Constants.Configuration.ProjectConfigFileName)));

      var postInstallScript = Path.Combine(context.UninstallDirectory, "content\\postinstall.xml");

      using (var streamWriter = new StreamWriter(postInstallScript, false, Encoding.UTF8))
      {
        var output = new XmlTextWriter(streamWriter)
        {
          Formatting = Formatting.Indented
        };

        output.WriteStartElement("actions");

        if (context.AddedItems.Any())
        {
          output.WriteStartElement("deleteItems");

          foreach (var item in context.AddedItems)
          {
            output.WriteElementString("item", item);
          }

          output.WriteEndElement();
        }

        if (context.AddedFiles.Any())
        {
          output.WriteStartElement("deleteFiles");

          foreach (var item in context.AddedFiles)
          {
            output.WriteElementString("item", item);
          }

          output.WriteEndElement();
        }

        output.WriteEndElement();
      }

      var nuspecFileName = Path.Combine(context.UninstallDirectory, "Uninstall.nuspec");
      this.BuildNuspecFile(nuspecFileName);
      this.BuildNupkgFile(context, nuspecFileName);
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
