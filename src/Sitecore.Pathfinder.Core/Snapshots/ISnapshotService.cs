// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots.Directives;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface ISnapshotService
    {
        [NotNull, ItemNotNull]
        IEnumerable<ISnapshotDirective> Directives { get; }

        [NotNull]
        ITextNode LoadIncludeFile([NotNull] SnapshotParseContext snapshotParseContext, [NotNull] ISnapshot snapshot, [NotNull] string includeFileName);

        [NotNull]
        ISnapshot LoadSnapshot([NotNull] SnapshotParseContext snapshotParseContext, [NotNull] ISourceFile sourceFile);

        [NotNull]
        ISnapshot LoadSnapshot([NotNull] IProjectBase project, [NotNull] ISourceFile sourceFile, [NotNull] PathMappingContext pathMappingContext);
    }
}
