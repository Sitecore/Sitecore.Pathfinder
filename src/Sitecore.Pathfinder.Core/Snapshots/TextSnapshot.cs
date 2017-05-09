// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Snapshots
{
    [Export]
    public class TextSnapshot : Snapshot, ITextSnapshot
    {
        [ImportingConstructor]
        public TextSnapshot([NotNull] ISnapshotService snapshotService)
        {
            SnapshotService = snapshotService;
        }

        public string ParseError { get; protected set; } = string.Empty;

        public TextSpan ParseErrorTextSpan { get; protected set; } = TextSpan.Empty;

        public virtual ITextNode Root { get; protected set; }

        [NotNull]
        protected ISnapshotService SnapshotService { get; }

        public virtual bool ValidateSchema(IParseContext context)
        {
            return true;
        }

        public override ISnapshot With(ISourceFile sourceFile)
        {
            base.With(sourceFile);

            Root = new SnapshotTextNode(this);

            return this;
        }
    }
}
