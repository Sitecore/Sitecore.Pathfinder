// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Data.Items;

namespace Sitecore.Data.Links
{
    public class LinkDatabase
    {
        public void Rebuild([NotNull] Database database)
        {
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<Item> GetReferrers([NotNull] Item item)
        {
        }
    }
}