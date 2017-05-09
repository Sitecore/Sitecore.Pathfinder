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
        public string Link => _item["__Help link"];

        [NotNull]
        public string Text => _item["__Long description"];

        [NotNull]
        public string ToolTip => _item["__Short description"];
    }
}
