// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Items
{
    public class ItemAppearance
    {
        [NotNull]
        private readonly Item _item;

        public ItemAppearance([NotNull] Item item)
        {
            _item = item;
        }

        [NotNull]
        public string DisplayName => _item[Constants.Fields.DisplayName];

        [NotNull]
        public string Icon => _item.Icon;

        [NotNull]
        public string ReadOnly => _item[Constants.Fields.ReadOnly];

        public int SortOrder => int.TryParse(_item[Constants.Fields.Sortorder], out int sortorder) ? sortorder : 0;
    }
}
