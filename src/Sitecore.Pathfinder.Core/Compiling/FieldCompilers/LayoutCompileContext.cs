// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    public class LayoutCompileContext
    {
        public LayoutCompileContext([NotNull] IFieldCompileContext compileContext, [NotNull] IFileSystemService fileSystem, [NotNull] Field field, [NotNull] ITextSnapshot snapshot)
        {
            CompileContext = compileContext;
            Field = field;
            FileSystem = fileSystem;
            Snapshot = snapshot;
        }

        [NotNull]
        public IFieldCompileContext CompileContext { get; }

        [NotNull]
        public Field Field { get; }

        [NotNull]
        public IFileSystemService FileSystem { get; }

        [NotNull]
        public ITextSnapshot Snapshot { get; }
    }
}
