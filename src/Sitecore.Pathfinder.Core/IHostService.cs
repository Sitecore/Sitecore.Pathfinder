// // © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Diagnostics;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Tasks;

namespace Sitecore.Pathfinder
{
    public interface IHostService
    {
        [NotNull]
        IConfiguration Configuration { get; }

        [NotNull]
        IFactory Factory { get; }

        [CanBeNull]
        Stopwatch Stopwatch { get; }

        [NotNull]
        T GetTaskRunner<T>() where T : ITaskRunner;

        [NotNull]
        IHostService With([CanBeNull] Stopwatch stopwatch);
    }
}
