// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.ProjectTrees;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Projects
{
    [Export(typeof(IProjectService))]
    public class ProjectService : IProjectService
    {
        [ImportingConstructor]
        public ProjectService([NotNull] IConfiguration configuration, [NotNull] IFactoryService factory, [NotNull] ExportFactory<IProject> projectFactory, [NotNull] ExportFactory<IProjectTree> projectTreeFactory)
        {
            Configuration = configuration;
            Factory = factory;
            ProjectFactory = projectFactory;
            ProjectTreeFactory = projectTreeFactory;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull]
        protected ExportFactory<IProject> ProjectFactory { get; }

        [NotNull]
        protected ExportFactory<IProjectTree> ProjectTreeFactory { get; }

        public virtual IProject LoadProject(ProjectOptions projectOptions, IEnumerable<string> sourceFiles)
        {
            return ProjectFactory.New().With(projectOptions, sourceFiles);
        }

        public virtual IProject LoadProjectFromConfiguration()
        {
            var projectDirectory = Configuration.GetProjectDirectory();
            var databaseName = Configuration.GetString(Constants.Configuration.Database);

            var projectOptions = Factory.ProjectOptions(projectDirectory, databaseName);

            projectOptions.LoadStandardTemplateFields(Configuration);
            projectOptions.LoadTokens(Configuration);

            var projectTree = GetProjectTree(projectOptions);
            var sourceFiles = projectTree.GetSourceFiles();

            return LoadProject(projectOptions, sourceFiles);
        }

        public virtual IProject LoadProjectFromNewHost(string projectDirectory)
        {
            var host = new Startup().AsInteractive().WithProjectDirectory(projectDirectory).Start();
            if (host == null)
            {
                return null;
            }

            var context = host.CompositionService.Resolve<IBuildContext>().With(() =>
            {
                var projectService = host.CompositionService.Resolve<IProjectService>();
                return projectService.LoadProjectFromConfiguration();
            });

            return context.Project;
        }

        [NotNull]
        protected virtual IProjectTree GetProjectTree([NotNull] ProjectOptions projectOptions)
        {
            return ProjectTreeFactory.New().With(Configuration.GetToolsDirectory(), projectOptions.ProjectDirectory);
        }
    }
}
