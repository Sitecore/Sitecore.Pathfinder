namespace Sitecore.Pathfinder.Projects
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.CompositionServiceExtensions;
  using Sitecore.Pathfinder.Extensions.ConfigurationExtensions;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.TextDocuments;

  [Export(typeof(IProjectService))]
  public class ProjectService : IProjectService
  {
    [ImportingConstructor]
    public ProjectService([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration)
    {
      this.CompositionService = compositionService;
      this.Configuration = configuration;
    }

    [NotNull]
    protected ICompositionService CompositionService { get; }

    [NotNull]
    protected IConfiguration Configuration { get; }

    public IProject LoadProjectFromConfiguration()
    {
      // todo: refactor this
      var projectDirectory = PathHelper.Combine(this.Configuration.GetString(Pathfinder.Constants.Configuration.SolutionDirectory), this.Configuration.GetString(Pathfinder.Constants.Configuration.ProjectDirectory));
      var databaseName = this.Configuration.GetString(Pathfinder.Constants.Configuration.Database);

      var ignoreFileNames = this.Configuration.GetList(Pathfinder.Constants.Configuration.IgnoreFileNames).ToList();
      var ignoreDirectories = this.Configuration.GetList(Pathfinder.Constants.Configuration.IgnoreDirectories).ToList();
      ignoreDirectories.Add(Path.GetFileName(this.Configuration.GetString(Pathfinder.Constants.Configuration.ToolsDirectory)));

      var project = this.CompositionService.Resolve<Project>().With(projectDirectory, databaseName);

      this.LoadExternalReferences(project);

      this.LoadProjectItems(project, ignoreDirectories, ignoreFileNames);

      return project;
    }

    protected virtual void LoadExternalReferences([NotNull] IProject project)
    {
      foreach (var pair in this.Configuration.GetSubKeys("external-references"))
      {
        var external = new ExternalReferenceItem(project, pair.Key, Document.Empty)
        {
          ItemIdOrPath = pair.Key, 
          ItemName = Path.GetFileName(pair.Key) ?? string.Empty
        };

        project.AddOrMerge(external);

        var value = this.Configuration.Get("external-references:" + pair.Key);
        if (string.IsNullOrEmpty(value))
        {
          continue;
        }

        external = new ExternalReferenceItem(project, value, Document.Empty)
        {
          ItemIdOrPath = value, 
          ItemName = Path.GetFileName(value)
        };
        project.AddOrMerge(external);
      }
    }

    protected virtual void LoadProjectItems([NotNull] Project project, [NotNull] IEnumerable<string> ignoreDirectories, [NotNull] IEnumerable<string> ignoreFileNames)
    {
      var visitor = this.CompositionService.Resolve<ProjectDirectoryVisitor>().With(ignoreDirectories, ignoreFileNames);
      visitor.Visit(project);
    }
  }
}
