// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Diagnostics;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Tasks;

namespace Sitecore.Pathfinder
{
    [Export(typeof(IHostService))]
    public class HostService : IHostService
    {
        [ImportingConstructor]
        public HostService([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService)
        {
            Configuration = configuration;
            CompositionService = compositionService;
        }

        public ICompositionService CompositionService { get; }

        public IConfiguration Configuration { get; }

        public Stopwatch Stopwatch { get; private set; }

        public T GetTaskRunner<T>() where T : ITaskRunner => CompositionService.Resolve<T>();

        public IHostService With(Stopwatch stopwatch)
        {
            Stopwatch = stopwatch;
            return this;
        }
    }
}
