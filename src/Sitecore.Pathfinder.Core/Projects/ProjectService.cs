// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.ProjectTrees;

namespace Sitecore.Pathfinder.Projects
{
    [Export(typeof(IProjectService))]
    public class ProjectService : IProjectService
    {
        [ImportingConstructor]
        public ProjectService([NotNull] IConfiguration configuration, [NotNull] IFactoryService factory, [NotNull] ExportFactory<IProjectTree> projectTreeFactory)
        {
            Configuration = configuration;
            Factory = factory;
            ProjectTreeFactory = projectTreeFactory;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull]
        protected ExportFactory<IProjectTree> ProjectTreeFactory { get; }

        public IProject LoadProjectFromConfiguration()
        {
            var projectDirectory = Configuration.GetProjectDirectory();

            return LoadProjectFromDirectory(projectDirectory);
        }

        [NotNull]
        public IProject LoadProjectFromDirectory([NotNull] string projectDirectory)
        {
            var databaseName = Configuration.GetString(Constants.Configuration.Database);

            var projectOptions = Factory.ProjectOptions(projectDirectory, databaseName);

            LoadStandardTemplateFields(projectOptions);
            LoadTokens(projectOptions);

            var projectTree = GetProjectTree(projectOptions);

            return projectTree.GetProject(projectOptions);
        }

        public virtual IProjectTree GetProjectTree(ProjectOptions projectOptions)
        {
            return ProjectTreeFactory.New().With(Configuration.GetString(Constants.Configuration.ToolsDirectory), projectOptions.ProjectDirectory);
        }

        protected virtual void LoadStandardTemplateFields([NotNull] ProjectOptions projectOptions)
        {
            foreach (var pair in Configuration.GetSubKeys(Constants.Configuration.StandardTemplateFields))
            {
                projectOptions.StandardTemplateFields.Add(pair.Key);

                var value = Configuration.GetString(Constants.Configuration.StandardTemplateFields + ":" + pair.Key);
                if (!string.IsNullOrEmpty(value))
                {
                    projectOptions.StandardTemplateFields.Add(value);
                }
            }
        }
        protected virtual void LoadTokens([NotNull] ProjectOptions projectOptions)
        {
            foreach (var pair in Configuration.GetSubKeys(Constants.Configuration.SearchAndReplaceTokens))
            {
                var value = Configuration.GetString(Constants.Configuration.SearchAndReplaceTokens + ":" + pair.Key);
                projectOptions.Tokens[pair.Key] = value;

            }
        }
    }
}
