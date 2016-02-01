// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Web;
using Sitecore.Data.Items;

namespace Sitecore.Mvc.Helpers
{
    public class SitecoreHelper
    {
        [NotNull]
        public virtual HtmlString Field(string fieldName, Item item)
        {
            throw new NotImplementedException();
        }

        public HtmlString Placeholder([NotNull] string placeHolderName)
        {
            throw new NotImplementedException();
        }

        [NotNull]
        public virtual HtmlString ViewRendering(string path)
        {
            throw new NotImplementedException();
        }
    }
}
