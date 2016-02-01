// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Data.Items;

namespace Sitecore.Data.Links
{
    public class LinkDatabase
    {
        [NotNull]
        public ItemLink[] GetReferrers([NotNull] Item item)
        {
            throw new NotImplementedException();
        }

        public void Rebuild([NotNull] Database database)
        {
            throw new NotImplementedException();
        }
    }
}
