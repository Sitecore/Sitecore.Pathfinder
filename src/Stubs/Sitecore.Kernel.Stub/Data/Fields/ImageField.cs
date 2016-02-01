// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Data.Items;

namespace Sitecore.Data.Fields
{
    public class ImageField : CustomField
    {
        public ImageField([NotNull] Field innerField) : base(innerField)
        {
        }

        [CanBeNull]
        public Item MediaItem
        {
            get { throw new NotImplementedException(); }
        }
    }
}
