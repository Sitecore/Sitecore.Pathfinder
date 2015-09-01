// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.Items
{
    public class ItemParseContext
    {
        public ItemParseContext([NotNull] IParseContext parseContext, [NotNull] ItemParser parser, [NotNull] string parentItemPath)
        {
            ParseContext = parseContext;
            Parser = parser;
            ParentItemPath = parentItemPath;
        }

        [NotNull]
        public string ParentItemPath { get; }

        [NotNull]
        public IParseContext ParseContext { get; }

        [NotNull]
        public ItemParser Parser { get; }

        [NotNull]
        public ITextSnapshot Snapshot => (ITextSnapshot)ParseContext.Snapshot;
    }
}
