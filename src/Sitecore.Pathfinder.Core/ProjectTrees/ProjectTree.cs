// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Configuration;
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
        [FactoryConstructor]
        public ProjectTree([NotNull] IConfiguration configuration, [NotNull] IFactory factory, [NotNull] IFileSystemService fileSystem, [NotNull] IPipelineService pipelines)
        {
            Configuration = configuration;
            Factory = factory;
            FileSystem = fileSystem;
            Pipelines = pipelines;

            PathMatcher = Factory.PathMatcher(Configuration.GetString(Constants.Configuration.Files.Include), Configuration.GetString(Constants.Configuration.Files.Exclude));
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
                    var root = Factory.DirectoryProjectTreeItem(this, ProjectDirectory);
                    yield return root;
                }
            }
        }

        public string ToolsDirectory { get; private set; } = string.Empty;

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactory Factory { get; }

        [NotNull]
        protected PathMatcher PathMatcher { get; }

        public virtual IEnumerable<string> GetSourceFiles()
        {
            var sourceFileNames = new List<string>();

            var visitor = Factory.ProjectTreeVisitor();
            visitor.Visit(this, sourceFileNames);

            sourceFileNames.Reverse();

            return sourceFileNames;
        }

        public virtual bool IsDirectoryIncluded(string directory) => !PathMatcher.IsExcluded(directory);

        public virtual bool IsFileIncluded(string fileName) => PathMatcher.IsMatch(fileName);

        public virtual IProjectTree With(string toolsDirectory, string projectDirectory)
        {
            ToolsDirectory = toolsDirectory;
            ProjectDirectory = projectDirectory;

            return this;
        }
    }
}
