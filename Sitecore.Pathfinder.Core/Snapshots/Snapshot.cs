// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: ProjectFileName: {SourceFile.ProjectFileName}")]
    public class Snapshot : ISnapshot
    {
        [NotNull]
        public static readonly ISnapshot Empty = new Snapshot(Snapshots.SourceFile.Empty);

        public Snapshot([NotNull] ISourceFile sourceFile)
        {
            SourceFile = sourceFile;
        }

        public bool IsModified { get; set; }

        public ISourceFile SourceFile { get; }

        public virtual ITextNode GetJsonChildTextNode(ITextNode textNode, string name)
        {
            // overwritten in JsonTextSnapshot to find the appropriate text node
            return textNode;
        }

        public virtual void SaveChanges()
        {
            throw new InvalidOperationException("Cannot save file: " + SourceFile.FileName);
        }
    }
}
