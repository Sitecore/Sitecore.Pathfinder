namespace Sitecore.Pathfinder.Projects
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Checking;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions;
  using Sitecore.Pathfinder.IO;

  [Export(typeof(IProjectService))]
  public class ProjectService : IProjectService
  {
    [ImportingConstructor]
    public ProjectService([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] ICheckerService checker)
    {
      this.CompositionService = compositionService;
      this.Configuration = configuration;
      this.Checker = checker;
    }

    [NotNull]
    protected ICheckerService Checker { get; set; }

    [NotNull]
    protected ICompositionService CompositionService { get; }

    [NotNull]
    protected IConfiguration Configuration { get; }

    public IProject LoadProjectFromConfiguration()
    {
      var projectOptions = this.CreateProjectOptions();

      this.LoadExternalReferences(projectOptions);
      this.LoadRemapFileDirectories(projectOptions);

      var sourceFileNames = new List<string>();
      this.LoadSourceFileNames(projectOptions, sourceFileNames);

      var project = this.CompositionService.Resolve<IProject>().Load(projectOptions, sourceFileNames);

      return project;
    }

    [NotNull]
    protected virtual ProjectOptions CreateProjectOptions()
    {
      var projectDirectory = PathHelper.Combine(this.Configuration.GetString(Pathfinder.Constants.Configuration.SolutionDirectory), this.Configuration.GetString(Pathfinder.Constants.Configuration.ProjectDirectory));
      var databaseName = this.Configuration.GetString(Pathfinder.Constants.Configuration.Database);

      return new ProjectOptions(projectDirectory, databaseName);
    }

    protected virtual void LoadExternalReferences([NotNull] ProjectOptions projectOptions)
    {
      foreach (var pair in this.Configuration.GetSubKeys(Pathfinder.Constants.Configuration.ExternalReferences))
      {
        projectOptions.ExternalReferences.Add(pair.Key);

        var value = this.Configuration.Get(Pathfinder.Constants.Configuration.ExternalReferences + ":" + pair.Key);
        if (!string.IsNullOrEmpty(value))
        {
          projectOptions.ExternalReferences.Add(value);
        }
      }
    }

    protected virtual void LoadRemapFileDirectories([NotNull] ProjectOptions projectOptions)
    {
      foreach (var pair in this.Configuration.GetSubKeys(Pathfinder.Constants.Configuration.RemapFileDirectories))
      {
        var value = this.Configuration.Get(Pathfinder.Constants.Configuration.RemapFileDirectories + ":" + pair.Key);
        projectOptions.RemapFileDirectories[pair.Key] = value;
      }
    }

    protected virtual void LoadSourceFileNames([NotNull] ProjectOptions projectOptions, [NotNull] ICollection<string> sourceFileNames)
    {
      var ignoreFileNames = this.Configuration.GetList(Pathfinder.Constants.Configuration.IgnoreFileNames).ToList();
      var ignoreDirectories = this.Configuration.GetList(Pathfinder.Constants.Configuration.IgnoreDirectories).ToList();
      ignoreDirectories.Add(Path.GetFileName(this.Configuration.GetString(Pathfinder.Constants.Configuration.ToolsDirectory)));

      var visitor = this.CompositionService.Resolve<ProjectDirectoryVisitor>().With(ignoreDirectories, ignoreFileNames);
      visitor.Visit(projectOptions, sourceFileNames);
    }
  }
}
