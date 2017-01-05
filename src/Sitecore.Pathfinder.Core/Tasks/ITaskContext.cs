// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Tasks
{
    public interface ITaskContext
    {
        [NotNull]
        IConfiguration Configuration { get; }

        [NotNull]
        IConsoleService Console { get; }

        int ErrorCode { get; set; }

        [NotNull]
        IFileSystemService FileSystem { get; }

        bool IsAborted { get; set; }

        [NotNull]
        ITraceService Trace { get; }
    }
}
