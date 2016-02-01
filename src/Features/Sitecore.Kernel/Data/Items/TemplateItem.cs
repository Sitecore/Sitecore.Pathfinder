// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Data.Items
{
    public class TemplateItem
    {
        public TemplateItem([NotNull] Item innerItem)
        {
            InnerItem = innerItem;
        }

        [NotNull]
        public Item InnerItem { get; }
    }
}
