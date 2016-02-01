// © 2015 Sitecore Corporation A/S. All rights reserved.
namespace Sitecore.Data.Items
{
    public class ItemAppearance
    {
        public int Sortorder { get; set; }

        [NotNull]
        public string Icon { get; set; }

        [NotNull]
        public string DisplayName { get; private set; }
    }
}