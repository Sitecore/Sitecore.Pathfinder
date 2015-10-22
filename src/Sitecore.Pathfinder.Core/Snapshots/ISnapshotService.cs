// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface ISnapshotService
    {
        [NotNull]
        ITextNode LoadIncludeFile([NotNull] ISnapshot snapshot, [NotNull] string includeFileName, [NotNull] SnapshotParseContext parseContext);

        [NotNull]
        ISnapshot LoadSnapshot([NotNull] ISourceFile sourceFile, [NotNull] SnapshotParseContext parseContext);
    }
}
