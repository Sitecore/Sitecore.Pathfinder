// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Data.Items;

namespace Sitecore.Data.Managers
{
    public static class ItemManager
    {
        [CanBeNull]
        public static Item AddFromTemplate([NotNull] string itemName, [NotNull] ID templateID, Item destination)
        {
            throw new NotImplementedException();
        }                                                                                       

        [CanBeNull]
        public static Item AddFromTemplate([NotNull] string itemName, [NotNull] ID templateId, [NotNull] Item destination, [NotNull] ID newItemId)
        {
            throw new NotImplementedException();
        }
    }
}
