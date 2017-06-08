// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Diagnostics;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Tasks;

namespace Sitecore.Pathfinder
{
    [Export(typeof(IHostService))]
    public class HostService : IHostService
    {
        [ImportingConstructor]
        public HostService([NotNull] IConfiguration configuration, [NotNull] IFactory factory)
        {
            Configuration = configuration;
            Factory = factory;
        }

        public IConfiguration Configuration { get; }

        public IFactory Factory { get; }

        public Stopwatch Stopwatch { get; private set; }

        public T GetTaskRunner<T>() where T : ITaskRunner => Factory.Resolve<T>();

        public IHostService With(Stopwatch stopwatch)
        {
            Stopwatch = stopwatch;
            return this;
        }
    }
}
