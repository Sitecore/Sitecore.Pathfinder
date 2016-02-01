// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Data.Items;

namespace Sitecore.Data.Query
{
    public class Query
    {
        public Query([NotNull] string queryText)
        {
            throw new NotImplementedException();
        }

        public int Max
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [CanBeNull]
        public object Execute([NotNull] Item getRootItem)
        {
            throw new NotImplementedException();
        }
    }
}
