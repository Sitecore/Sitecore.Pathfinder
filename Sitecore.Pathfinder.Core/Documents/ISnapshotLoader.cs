// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Documents
{
    public interface ISnapshotLoader
    {
        double Priority { get; }

        bool CanLoad([NotNull] ISnapshotService snapshotService, [NotNull] IProject project, [NotNull] ISourceFile sourceFile);

        [NotNull]
        ISnapshot Load([NotNull] ISnapshotService snapshotService, [NotNull] IProject project, [NotNull] ISourceFile sourceFile);
    }
}
