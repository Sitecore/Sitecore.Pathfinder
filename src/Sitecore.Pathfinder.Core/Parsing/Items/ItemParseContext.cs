// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.Items
{
    public class ItemParseContext
    {
        public ItemParseContext([NotNull] IParseContext parseContext, [NotNull] ItemParser parser, [NotNull] string databaseName, [NotNull] string parentItemPath, bool isImport)
        {
            ParseContext = parseContext;
            Parser = parser;
            DatabaseName = databaseName;
            ParentItemPath = parentItemPath;
            IsImport = isImport;
        }

        [NotNull]
        public string DatabaseName { get; }

        public bool IsImport { get; }

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
