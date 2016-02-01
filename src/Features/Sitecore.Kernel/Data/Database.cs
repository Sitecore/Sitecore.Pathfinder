// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Data.Indexing;
using Sitecore.Data.Items;

namespace Sitecore.Data
{
    public class Database
    {
        [NotNull]
        public DataIndexes Indexes { get; private set; }

        [NotNull]
        public string Name { get; private set; }

        [CanBeNull]
        public Item CreateItemPath([NotNull] string parentItemPath)
        {
            return null;
        }

        [CanBeNull]
        public Item GetItem([NotNull] string itemPath)
        {
            return null;
        }

        [CanBeNull]
        public Item GetItem([NotNull] ID id)
        {
            return null;
        }

        [NotNull]
        public Item GetRootItem()
        {
            throw new NotImplementedException();
        }
    }
}
