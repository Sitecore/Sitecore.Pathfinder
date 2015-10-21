// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Snapshots
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TextSnapshot : Snapshot, ITextSnapshot
    {
        public string ParseError { get; protected set; } = string.Empty;

        public TextSpan ParseErrorTextSpan { get; protected set; } = TextSpan.Empty;

        public virtual ITextNode Root { get; protected set; }

        public virtual void ValidateSchema(IParseContext context)
        {
        }

        public override ISnapshot With(ISourceFile sourceFile)
        {
            base.With(sourceFile);

            Root = new SnapshotTextNode(this);

            return this;
        }
    }
}
