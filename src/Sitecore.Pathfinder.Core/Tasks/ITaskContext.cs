// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Tasks
{
    public interface ITaskContext
    {
        [NotNull]
        IConfiguration Configuration { get; }

        [NotNull]
        IConsoleService Console { get; }

        int ErrorCode { get; set; }

        bool IsAborted { get; set; }

        [NotNull]
        ITraceService Trace { get; }
    }
}
