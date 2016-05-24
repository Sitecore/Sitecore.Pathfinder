// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.ProjectTrees
{
    [Export(typeof(IProjectTree))]
    public class ProjectTree : IProjectTree
    {
        [CanBeNull, ItemNotNull]
        private HashSet<string> _ignoreDirectories;

        [CanBeNull, ItemNotNull]
        private HashSet<string> _ignoreFileNames;

        [ImportingConstructor]
        public ProjectTree([NotNull] IConfiguration configuration, [NotNull] IFileSystemService fileSystem, [NotNull] IPipelineService pipelines, [NotNull] ExportFactory<IProject> projectFactory, [NotNull] ExportFactory<ProjectTreeVisitor> projectTreeVisitorFactory)
        {
            Configuration = configuration;
            FileSystem = fileSystem;
            Pipelines = pipelines;
            ProjectFactory = projectFactory;
            ProjectTreeVisitorFactory = projectTreeVisitorFactory;
        }

        public IFileSystemService FileSystem { get; }

        public HashSet<string> IgnoreDirectories => _ignoreDirectories ?? (_ignoreDirectories = new HashSet<string>(Configuration.GetCommaSeparatedStringList(Constants.Configuration.ProjectWebsiteMappings.IgnoreDirectories)));

        public HashSet<string> IgnoreFileNames => _ignoreFileNames ?? (_ignoreFileNames = GetIgnoreDirectories());

        public IPipelineService Pipelines { get; }

        [NotNull]
        protected ExportFactory<IProject> ProjectFactory { get; }

        [NotNull]
        protected ExportFactory<ProjectTreeVisitor> ProjectTreeVisitorFactory { get; }

        public string ProjectDirectory { get; private set; } = string.Empty;

        public IEnumerable<IProjectTreeItem> Roots
        {
            get
            {
                if (FileSystem.DirectoryExists(ProjectDirectory))
                {
                    var root = new DirectoryProjectTreeItem(this, ProjectDirectory);
                    yield return root;
                }
            }
        }

        public string ToolsDirectory { get; private set; } = string.Empty;

        [NotNull]
        protected IConfiguration Configuration { get; }

        public virtual IProject GetProject(ProjectOptions projectOptions)
        {
            var sourceFileNames = new List<string>();

            var visitor = ProjectTreeVisitorFactory.New();
            visitor.Visit(this, sourceFileNames);

            sourceFileNames.Reverse();

            return ProjectFactory.New().With(projectOptions, sourceFileNames);
        }

        public virtual IProjectTree With(string toolsDirectory, string projectDirectory)
        {
            ToolsDirectory = toolsDirectory;
            ProjectDirectory = projectDirectory;

            return this;
        }

        [NotNull, ItemNotNull]
        protected virtual HashSet<string> GetIgnoreDirectories()
        {
            var ignoreDirectories = Configuration.GetCommaSeparatedStringList(Constants.Configuration.ProjectWebsiteMappings.IgnoreFileNames).ToList();

            ignoreDirectories.Add(Path.GetFileName(Configuration.GetString(Constants.Configuration.ToolsDirectory)));

            return new HashSet<string>(ignoreDirectories);
        }
    }
}
