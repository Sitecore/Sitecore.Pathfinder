// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Data.Items
{
    public class TemplateFieldItem : CustomItemBase
    {
        public TemplateFieldItem([NotNull] Item innerItem) : base(innerItem)
        {
        }

        [NotNull]
        public string Source
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public string Type
        {
            get { throw new NotImplementedException(); }
        }
    }
}
