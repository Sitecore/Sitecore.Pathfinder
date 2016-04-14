// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Diagnostics;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Tasks;

namespace Sitecore.Pathfinder
{
    public interface IAppService
    {
        [NotNull]
        ICompositionService CompositionService { get; }

        [NotNull]
        IConfiguration Configuration { get; }

        [NotNull]
        string ProjectDirectory { get; }

        [CanBeNull]
        Stopwatch Stopwatch { get; }

        [NotNull]
        string ToolsDirectory { get; }

        [NotNull]
        T GetTaskRunner<T>() where T : ITaskRunner;
    }
}
