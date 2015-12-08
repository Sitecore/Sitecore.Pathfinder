// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data;

namespace Sitecore.Pathfinder.Serializing
{
    public class QueuedSerializationItem
    {
        public QueuedSerializationItem([Diagnostics.NotNull] Database database, [Diagnostics.NotNull] ID itemId)
        {
            Database = database;
            ItemId = itemId;
        }

        [Diagnostics.NotNull]
        public Database Database { get; }

        [Diagnostics.NotNull]
        public ID ItemId { get; }
    }
}
