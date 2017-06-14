// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Parsing.Items
{
    public class ItemParseContext
    {
        [FactoryConstructor]
        public ItemParseContext([NotNull] IParseContext parseContext, [NotNull] ItemParser parser, [NotNull] IDatabase database, [NotNull] string parentItemPath, bool isImport)
        {
            ParseContext = parseContext;
            Parser = parser;
            Database = database;
            ParentItemPath = parentItemPath;
            IsImport = isImport;
        }

        [NotNull]
        public IDatabase Database { get; }

        public bool IsImport { get; }

        [NotNull]
        public string ParentItemPath { get; }

        [NotNull]
        public IParseContext ParseContext { get; }

        [NotNull]
        public ItemParser Parser { get; }

        public int Sortorder { get; private set; }

        [NotNull]
        public ItemParseContext With(int sortorder)
        {
            Sortorder = sortorder;
            return this;
        }
    }
}
