// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface ISnapshotService
    {
        [NotNull]
        ISnapshot LoadSnapshot([NotNull] SnapshotParseContext snapshotParseContext, [NotNull] ISourceFile sourceFile);

        [NotNull]
        ISnapshot LoadSnapshot([NotNull] IProjectBase project, [NotNull] ISourceFile sourceFile, [NotNull] PathMappingContext pathMappingContext);
    }
}
