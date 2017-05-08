// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.ProjectTrees
{
    [Export(typeof(IProjectTree))]
    public class ProjectTree : IProjectTree
    {
        [ImportingConstructor]
        public ProjectTree([NotNull] IConfiguration configuration, [NotNull] IFileSystemService fileSystem, [NotNull] IPipelineService pipelines, [NotNull] ExportFactory<ProjectTreeVisitor> projectTreeVisitorFactory)
        {
            Configuration = configuration;
            FileSystem = fileSystem;
            Pipelines = pipelines;
            ProjectTreeVisitorFactory = projectTreeVisitorFactory;

            PathMatcher = new PathMatcher(Configuration.GetString(Constants.Configuration.Files.Include), Configuration.GetString(Constants.Configuration.Files.Exclude));
        }

        public IFileSystemService FileSystem { get; }

        public IPipelineService Pipelines { get; }

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

        [NotNull]
        protected PathMatcher PathMatcher { get; }

        [NotNull]
        protected ExportFactory<ProjectTreeVisitor> ProjectTreeVisitorFactory { get; }

        public virtual IEnumerable<string> GetSourceFiles()
        {
            var sourceFileNames = new List<string>();

            var visitor = ProjectTreeVisitorFactory.New();
            visitor.Visit(this, sourceFileNames);

            sourceFileNames.Reverse();

            return sourceFileNames;
        }

        public virtual bool IsDirectoryIncluded(string directory)
        {
            return true;
        }

        public virtual bool IsFileIncluded(string fileName)
        {
            return PathMatcher.IsMatch(fileName);
        }

        public virtual IProjectTree With(string toolsDirectory, string projectDirectory)
        {
            ToolsDirectory = toolsDirectory;
            ProjectDirectory = projectDirectory;

            return this;
        }
    }
}
