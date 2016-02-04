// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Data.Indexing;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace Sitecore.Data
{
    public class Database
    {
        [NotNull]
        public DataIndexes Indexes
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public string Name
        {
            get { throw new System.NotImplementedException(); }
        }

        [CanBeNull]
        public Item CreateItemPath([NotNull] string parentItemPath)
        {
            throw new NotImplementedException();
        }

        [CanBeNull]
        public Item CreateItemPath([NotNull] string parentItemPath, [NotNull] TemplateItem templateItem)
        {
            throw new NotImplementedException();
        }

        [NotNull]
        public Item CreateItemPath([NotNull] string parentItemPath, [NotNull] TemplateItem templateFolder, [NotNull] TemplateItem templateItem)
        {
            throw new NotImplementedException();
        }

        [CanBeNull]
        public Item GetItem([NotNull] string itemPath)
        {
            throw new NotImplementedException();
        }

        [CanBeNull]
        public Item GetItem([NotNull] ID id)
        {
            throw new NotImplementedException();
        }

        [CanBeNull]
        public Item GetItem([NotNull] ID id, Language language, [NotNull] Version version)
        {
            throw new NotImplementedException();
        }

        [NotNull]
        public Item GetRootItem()
        {
            throw new NotImplementedException();
        }
    }
}
