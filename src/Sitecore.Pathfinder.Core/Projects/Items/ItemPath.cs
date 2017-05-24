// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Projects.Items
{
    public class ItemPath
    {
        [NotNull]
        private readonly Item _item;

        [CanBeNull]
        private string _parentPath;

        public ItemPath([NotNull] Item item)
        {
            _item = item;
        }

        public bool IsBranch => _item.ItemIdOrPath.StartsWith("/sitecore/templates/branches/", StringComparison.OrdinalIgnoreCase);

        public bool IsStandardValuesHolder => _item.ItemName == "__Standard Values";

        [NotNull]
        public string ParentPath => _parentPath ?? (_parentPath = PathHelper.GetItemParentPath(_item.ItemIdOrPath));

        [NotNull]
        public string Path => _item.ItemIdOrPath;
    }
}
