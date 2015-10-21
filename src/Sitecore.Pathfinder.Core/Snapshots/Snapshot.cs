// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: ProjectFileName: {SourceFile.ProjectFileName}")]
    public class Snapshot : ISnapshot
    {
        [NotNull]
        public static readonly ISnapshot Empty = new Snapshot().With(Snapshots.SourceFile.Empty);

        public bool IsModified { get; set; }

        public ISourceFile SourceFile { get; private set; }

        public virtual void SaveChanges()
        {
            throw new InvalidOperationException("Cannot save file: " + SourceFile.AbsoluteFileName);
        }

        [NotNull]
        public virtual ISnapshot With([NotNull] ISourceFile sourceFile)
        {
            SourceFile = sourceFile;
            return this;
        }
    }
}
