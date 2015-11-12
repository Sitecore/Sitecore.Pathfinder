// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder
{
    public class AppService : IAppService
    {
        public AppService([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] string toolsDirectory, [NotNull] string projectDirectory)
        {
            Configuration = configuration;
            CompositionService = compositionService;
            ToolsDirectory = toolsDirectory;
            ProjectDirectory = projectDirectory;
        }

        public ICompositionService CompositionService { get; }

        public IConfiguration Configuration { get; }

        public string ProjectDirectory { get; }

        public string ToolsDirectory { get; }
    }
}
