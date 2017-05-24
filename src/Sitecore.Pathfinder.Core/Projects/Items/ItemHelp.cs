// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Items
{
    public class ItemHelp
    {
        [NotNull]
        private readonly Item _item;

        public ItemHelp([NotNull] Item item)
        {
            _item = item;
        }

        [NotNull]
        public string Link => _item[Constants.FieldNames.HelpLink];

        [NotNull]
        public string Text => _item[Constants.FieldNames.LongDescription];

        [NotNull]
        public string ToolTip => _item[Constants.FieldNames.ShortDescription];
    }
}
