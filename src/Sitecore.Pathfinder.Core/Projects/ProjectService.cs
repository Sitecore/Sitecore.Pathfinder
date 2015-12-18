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
        public ProjectService([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] IFactoryService factory)
        {
            CompositionService = compositionService;
            Configuration = configuration;
            Factory = factory;
        }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactoryService Factory { get; }

        public IProject LoadProjectFromConfiguration()
        {
            var projectOptions = GetProjectOptions();

            var projectTree = GetProjectTree(projectOptions);

            return projectTree.GetProject(projectOptions);
        }

        public virtual ProjectOptions GetProjectOptions()
        {
            var projectDirectory = Configuration.GetString(Constants.Configuration.ProjectDirectory);
            var databaseName = Configuration.GetString(Constants.Configuration.Database);

            var projectOptions = Factory.ProjectOptions(projectDirectory, databaseName);

            LoadStandardTemplateFields(projectOptions);
            LoadTokens(projectOptions);

            return projectOptions;
        }

        public virtual IProjectTree GetProjectTree(ProjectOptions projectOptions)
        {
            var projectTree = CompositionService.Resolve<IProjectTree>().With(Configuration.GetString(Constants.Configuration.ToolsDirectory), projectOptions.ProjectDirectory);

            return projectTree;
        }

        protected virtual void LoadStandardTemplateFields([NotNull] ProjectOptions projectOptions)
        {
            foreach (var pair in Configuration.GetSubKeys(Constants.Configuration.StandardTemplateFields))
            {
                projectOptions.StandardTemplateFields.Add(pair.Key);

                var value = Configuration.Get(Constants.Configuration.StandardTemplateFields + ":" + pair.Key);
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
                var value = Configuration.Get(Constants.Configuration.SearchAndReplaceTokens + ":" + pair.Key);
                projectOptions.Tokens[pair.Key] = value;

            }
        }
    }
}
