// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitting
{
    public interface IEmitContext
    {
        [NotNull]
        IConfiguration Configuration { get; }

        [NotNull]
        IFileSystemService FileSystem { get; }

        bool ForceUpdate { get; }

        [NotNull]
        IProject Project { get; }

        [NotNull]
        ITraceService Trace { get; }

        [NotNull]
        IEmitContext With([NotNull] IProject project);
    }
}
