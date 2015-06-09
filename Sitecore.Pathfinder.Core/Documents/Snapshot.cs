// © 2015 Sitecore Corporation A/S. All rights reserved.

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

        public ISourceFile SourceFile { get; }
    }
}
