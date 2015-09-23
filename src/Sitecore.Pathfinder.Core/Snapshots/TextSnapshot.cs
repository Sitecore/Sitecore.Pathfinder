// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Snapshots
{
    public class TextSnapshot : Snapshot, ITextSnapshot
    {
        public TextSnapshot([NotNull] ISourceFile sourceFile) : base(sourceFile)
        {
            Root = new SnapshotTextNode(this);
        }

        public string ParseError { get; protected set; } = string.Empty;

        public TextSpan ParseErrorTextSpan { get; protected set; } = TextSpan.Empty;

        public virtual ITextNode Root { get; }

        public virtual void ValidateSchema(IParseContext context)
        {
        }
    }
}
