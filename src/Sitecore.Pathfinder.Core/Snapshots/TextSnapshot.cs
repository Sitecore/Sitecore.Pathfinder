// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Snapshots
{
    [Export]
    public class TextSnapshot : Snapshot, ITextSnapshot
    {
        [CanBeNull]
        private ITextNode _root;

        [ImportingConstructor]
        public TextSnapshot([NotNull] ISnapshotService snapshotService)
        {
            SnapshotService = snapshotService;
        }

        public string ParseError { get; protected set; } = string.Empty;

        public TextSpan ParseErrorTextSpan { get; protected set; } = TextSpan.Empty;

        public virtual ITextNode Root => _root ?? (_root = new SnapshotTextNode(this));

        [NotNull]
        protected ISnapshotService SnapshotService { get; }

        public virtual bool ValidateSchema(IParseContext context) => true;
    }
}
