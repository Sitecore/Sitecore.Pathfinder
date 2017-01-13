// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Extensibility
{
    public interface IExtensionContext
    {
        [NotNull]
        IConfiguration Configuration { get; }

        [NotNull]
        IFileSystemService FileSystem { get; }

        [NotNull]
        ITraceService Trace { get; }
    }
}
