// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Documents
{
    public class Snapshot : ISnapshot
    {
        public static readonly ISnapshot Empty = new Snapshot(Documents.SourceFile.Empty);

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
