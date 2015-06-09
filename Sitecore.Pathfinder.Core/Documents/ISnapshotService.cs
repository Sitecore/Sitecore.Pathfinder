// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Documents
{
    public interface ISnapshotService
    {
        [NotNull]
        ISnapshot LoadSnapshot([NotNull] IProject project, [NotNull] ISourceFile sourceFile);

        [NotNull]
        string ReplaceTokens([NotNull] IProject project, [NotNull] ISourceFile sourceFile, [NotNull] string contents);
    }
}
