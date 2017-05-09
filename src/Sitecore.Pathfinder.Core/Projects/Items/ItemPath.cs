// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Items
{
    public class ItemPath
    {
        [NotNull]
        private readonly Item _item;

        public ItemPath([NotNull] Item item)
        {
            _item = item;
        }

        [NotNull]
        public string ParentPath => _item.ParentItemPath;

        [NotNull]
        public string Path => _item.ItemIdOrPath;
    }
}
