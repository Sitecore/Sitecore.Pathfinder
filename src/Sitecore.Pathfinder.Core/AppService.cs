// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks;

namespace Sitecore.Pathfinder
{
    public class AppService : IAppService
    {
        public AppService([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] string toolsDirectory, [NotNull] string projectDirectory, [CanBeNull] Stopwatch stopwatch)
        {
            Configuration = configuration;
            CompositionService = compositionService;
            ToolsDirectory = toolsDirectory;
            ProjectDirectory = projectDirectory;
            Stopwatch = stopwatch;
        }

        public ICompositionService CompositionService { get; }

        public IConfiguration Configuration { get; }

        public string ProjectDirectory { get; }

        public Stopwatch Stopwatch { get; }

        public string ToolsDirectory { get; }

        public T GetTaskRunner<T>() where T : ITaskRunner
        {
            return (T)CompositionService.Resolve<T>().With(this);
        }
    }
}
