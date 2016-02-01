// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Data.Items
{
    public class TemplateSectionItem : CustomBaseItem
    {
        public TemplateSectionItem([NotNull] Item innerItem) : base(innerItem)
        {
        }

        [NotNull, ItemNotNull]
        public TemplateFieldItem[] GetFields()
        {
            throw new NotImplementedException();
        }
    }
}
