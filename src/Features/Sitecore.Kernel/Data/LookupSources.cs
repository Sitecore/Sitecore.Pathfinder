// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using Sitecore.Data.Items;

namespace Sitecore.Data
{
    public static class LookupSources
    {
        [NotNull]
        public static Database GetDatabase([NotNull] string source)
        {
            throw new NotImplementedException();
        }

        [ItemNotNull, NotNull]
        public static IEnumerable<Item> GetItems([NotNull] Item rootItem, [NotNull] string source)
        {
            yield break;
        }
    }
}
