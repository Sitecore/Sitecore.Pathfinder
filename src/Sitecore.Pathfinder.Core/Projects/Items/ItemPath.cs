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

        [NotNull]
        public string ParentPath => _parentPath ?? (_parentPath = PathHelper.GetItemParentPath(_item.ItemIdOrPath));

        [NotNull]
        public string Path => _item.ItemIdOrPath;
    }
}
