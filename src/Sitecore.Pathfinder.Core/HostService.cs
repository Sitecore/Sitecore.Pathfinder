// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Diagnostics;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks;

namespace Sitecore.Pathfinder
{
    public class HostService : IHostService
    {
        public HostService([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [CanBeNull] Stopwatch stopwatch)
        {
            Configuration = configuration;
            CompositionService = compositionService;
            Stopwatch = stopwatch;
        }

        public ICompositionService CompositionService { get; }

        public IConfiguration Configuration { get; }

        public Stopwatch Stopwatch { get; }

        public T GetTaskRunner<T>() where T : ITaskRunner
        {
            return CompositionService.Resolve<T>();
        }
    }
}
