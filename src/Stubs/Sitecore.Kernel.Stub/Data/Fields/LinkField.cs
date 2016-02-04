// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Data.Items;

namespace Sitecore.Data.Fields
{
    public class LinkField : CustomField
    {
        public LinkField([NotNull] Field innerField) : base(innerField)
        {
        }

        [CanBeNull]
        public Item TargetItem
        {
            get { throw new NotImplementedException(); }
        }
    }
}
