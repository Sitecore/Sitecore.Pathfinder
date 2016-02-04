// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Web.Mvc;

namespace Sitecore.Mvc.Presentation
{
    public class PageContext
    {
        [NotNull]
        public static PageContext Current
        {
            get { throw new System.NotImplementedException(); }
        }

        public HtmlHelper HtmlHelper
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}