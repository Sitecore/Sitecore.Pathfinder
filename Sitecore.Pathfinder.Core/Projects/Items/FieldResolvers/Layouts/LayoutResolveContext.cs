// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Items.FieldResolvers.Layouts
{
    public class LayoutResolveContext
    {
        public LayoutResolveContext(IFileSystemService fileSystem, [NotNull] Field field, [NotNull] ITextSnapshot snapshot)
        {
            Field = field;
            FileSystem = fileSystem;
            Snapshot = snapshot;
        }

        [NotNull]
        public Field Field { get; }

        [NotNull]
        public IFileSystemService FileSystem { get; }

        [NotNull]
        public ITextSnapshot Snapshot { get; }
    }
}
