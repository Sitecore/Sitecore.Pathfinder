namespace Sitecore.Pathfinder.Emitters
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.IO;
  using Sitecore.Pathfinder.Data;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.ConfigurationExtensions;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.SecurityModel;

  [Export]
  public class Emitter
  {
    [ImportingConstructor]
    public Emitter([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] IDataService dataService, [NotNull] ITraceService traceService, [NotNull] IFileSystemService fileSystem, [Sitecore.NotNull] IParseService parseService)
    {
      this.CompositionService = compositionService;
      this.Configuration = configuration;
      this.DataService = dataService;
      this.Trace = traceService;
      this.FileSystem = fileSystem;
      this.ParseService = parseService;
    }

    [NotNull]
    protected ICompositionService CompositionService { get; }

    protected IConfiguration Configuration { get; set; }

    [NotNull]
    protected IDataService DataService { get; }

    [NotNull]
    [ImportMany]
    protected IEnumerable<IEmitter> Emitters { get; private set; }

    [NotNull]
    protected IFileSystemService FileSystem { get; }

    [NotNull]
    protected IParseService ParseService { get; }

    [NotNull]
    protected ITraceService Trace { get; }

    public virtual void Start([NotNull] string projectDirectory)
    {
      this.LoadConfiguration(projectDirectory);

      this.Trace.ProjectDirectory = projectDirectory;

      // todo: change to abstract factory pattern
      var project = new Project(this.CompositionService, this.FileSystem, this.ParseService).Load(projectDirectory, "master");

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
      }
    }

    protected virtual void LoadConfiguration(string directory)
    {
      var configuration = this.Configuration as IConfigurationSourceRoot;
      if (configuration == null)
      {
        throw new ConfigurationException(ConsoleTexts.Text3000);
      }

      // add command line
      var commandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToArray();
      configuration.AddCommandLine(commandLineArgs);

      // set website path
      var solutionDirectory = FileUtil.MapPath(directory);
      configuration.Set(Pathfinder.Constants.SolutionDirectory, solutionDirectory);

      // add build config
      var websiteConfigFileName = PathHelper.Combine(solutionDirectory, configuration.Get(Pathfinder.Constants.ConfigFileName));
      if (File.Exists(websiteConfigFileName))
      {
        configuration.AddFile(websiteConfigFileName);
      }

      // set project path
      var projectDirectory = PathHelper.NormalizeFilePath(configuration.Get(Pathfinder.Constants.ProjectDirectory) ?? string.Empty).TrimStart('\\');
      configuration.Set(Pathfinder.Constants.ProjectDirectory, projectDirectory);
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