// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Diagnostics;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    [Export, DebuggerDisplay("\\{{GetType().Name,nq}\\}: ProjectFileName: {SourceFile.ProjectFileName}")]
    public class Snapshot : ISnapshot
    {
        [FactoryConstructor]
        [ImportingConstructor]
        public Snapshot()
        {
        }

        [NotNull]
        public static readonly ISnapshot Empty = new Snapshot().With(Snapshots.SourceFile.Empty);

        public ISourceFile SourceFile { get; private set; } = Snapshots.SourceFile.Empty;

        [NotNull]
        public virtual ISnapshot With([NotNull] ISourceFile sourceFile)
        {
            SourceFile = sourceFile;
            return this;
        }
    }
}
