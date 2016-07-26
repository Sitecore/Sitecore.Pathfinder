// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

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

        [NotNull]
        public int SortOrder
        {
            get
            {
                int sortorder;
                return int.TryParse(_item[Constants.Fields.Sortorder], out sortorder) ? sortorder : 0;
            }
        }
    }
}
