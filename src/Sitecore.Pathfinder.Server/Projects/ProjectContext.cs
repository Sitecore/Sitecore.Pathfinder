// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.ProjectTrees;
using Sitecore.Pathfinder.Serializing;

namespace Sitecore.Pathfinder.Projects
{
    public class ProjectContext : IProjectContext
    {
        public ProjectContext([Diagnostics.NotNull] IConfiguration configuration, [Diagnostics.NotNull] ICompositionService compositionService, [Diagnostics.NotNull] IProjectTree projectTree)
        {
            CompositionService = compositionService;
            Configuration = configuration;
            ProjectTree = projectTree;

            Name = Path.GetFileName(ProjectTree.ProjectDirectory);
            WebsiteSerializer = CompositionService.Resolve<WebsiteSerializer>().With(projectTree.ToolsDirectory, projectTree.ProjectDirectory);
        }

        public ICompositionService CompositionService { get; }

        public IConfiguration Configuration { get; }

        public string Name { get; }

        public IProjectTree ProjectTree { get; }

        public WebsiteSerializer WebsiteSerializer { get; }
    }
}
