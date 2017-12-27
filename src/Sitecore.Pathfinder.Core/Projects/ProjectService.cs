// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.ProjectTrees;

namespace Sitecore.Pathfinder.Projects
{
    [Export(typeof(IProjectService)), Shared]
    public class ProjectService : IProjectService
    {
        [FactoryConstructor, ImportingConstructor]
        public ProjectService([NotNull] IConfiguration configuration, [NotNull] IFactory factory)
        {
            Configuration = configuration;
            Factory = factory;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactory Factory { get; }

        public virtual IProject LoadProject(ProjectOptions projectOptions, IEnumerable<string> sourceFiles) => Factory.Project(projectOptions, sourceFiles);

        public virtual IProject LoadProjectFromConfiguration()
        {
            var databaseName = Configuration.GetString(Constants.Configuration.Database);

            var projectOptions = Factory.ProjectOptions(databaseName);

            projectOptions.LoadTokens(Configuration);

            var projectTree = GetProjectTree(projectOptions);
            var sourceFiles = projectTree.GetSourceFiles();

            return LoadProject(projectOptions, sourceFiles);
        }

        public virtual IProject LoadProjectFromNewHost(string projectDirectory)
        {
            var host = new Startup().WithProjectDirectory(projectDirectory).Start();
            if (host == null)
            {
                return null;
            }

            var context = host.Factory.BuildContext().With(() =>
            {
                var projectService = host.Factory.ProjectService();
                return projectService.LoadProjectFromConfiguration();
            });

            return context.LoadProject();
        }

        [NotNull]
        protected virtual IProjectTree GetProjectTree([NotNull] ProjectOptions projectOptions) => Factory.ProjectTree(Configuration.GetToolsDirectory(), Configuration.GetProjectDirectory());
    }
}
