// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Tasks
{
    public interface ITaskContext
    {
        [NotNull]
        ICompositionService CompositionService { get; }

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
