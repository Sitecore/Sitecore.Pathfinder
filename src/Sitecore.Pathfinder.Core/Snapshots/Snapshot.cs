// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    [Flags]
    public enum SnapshotCapabilities
    {
        None = 0x00,

        SupportTildeInFileNames = 0x01,

        SupportsTrueAndFalseForBooleanFields = 0x02,

        All = SupportTildeInFileNames | SupportsTrueAndFalseForBooleanFields
    }

    [Export, DebuggerDisplay("\\{{GetType().Name,nq}\\}: ProjectFileName: {SourceFile.ProjectFileName}")]
    public class Snapshot : ISnapshot
    {
        [NotNull]
        public static readonly ISnapshot Empty = new Snapshot().With(Snapshots.SourceFile.Empty);

        public Snapshot()
        {
            Capabilities = SnapshotCapabilities.All;
        }

        public SnapshotCapabilities Capabilities { get; protected set; }

        public ISourceFile SourceFile { get; private set; } = Snapshots.SourceFile.Empty;

        [NotNull]
        public virtual ISnapshot With([NotNull] ISourceFile sourceFile)
        {
            SourceFile = sourceFile;
            return this;
        }
    }
}
