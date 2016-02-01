// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Data.Items;

namespace Sitecore.Data.Managers
{
    public static class ItemManager
    {
        [CanBeNull]
        public static Item AddFromTemplate([NotNull] string name, [NotNull] TemplateID templateID, Item parent, [NotNull] ID id)
        {
            throw new NotImplementedException();
        }

        [CanBeNull]
        public static Item AddFromTemplate([NotNull] string name, [NotNull] ID templateID, [NotNull] Item parent, [NotNull] ID id)
        {
            throw new NotImplementedException();
        }
    }
}
