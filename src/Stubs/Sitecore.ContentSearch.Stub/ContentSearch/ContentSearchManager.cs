// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;

namespace Sitecore.ContentSearch
{
    public class ContentSearchManager
    {
        [NotNull, ItemNotNull]
        public static IEnumerable<ISearchIndex> Indexes
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public static ISearchIndex GetIndex([NotNull] string indexName)
        {
            throw new NotImplementedException();
        }
    }
}
