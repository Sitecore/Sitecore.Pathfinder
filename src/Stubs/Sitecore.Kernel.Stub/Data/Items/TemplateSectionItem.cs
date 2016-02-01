// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Data.Items
{
    public class TemplateSectionItem : CustomItemBase
    {
        public TemplateSectionItem([NotNull] Item innerItem) : base(innerItem)
        {
        }

        [NotNull]
        public TemplateFieldItem[] GetFields()
        {
            throw new NotImplementedException();
        }
    }
}
