﻿// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Data.Items
{
    public class TemplateItem : CustomItemBase
    {
        public TemplateItem([NotNull] Item innerItem) : base(innerItem)
        {
            throw new NotImplementedException();
        }

        [NotNull]
        public TemplateItem[] BaseTemplates
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public TemplateSectionItem[] GetSections()
        {
            throw new NotImplementedException();
        }

        [NotNull]
        public static implicit operator TemplateItem([NotNull] Item item)
        {
            throw new NotImplementedException();
        }

        [NotNull]
        public static implicit operator Item([NotNull] TemplateItem templateItem)
        {
            throw new NotImplementedException();
        }
    }
}
