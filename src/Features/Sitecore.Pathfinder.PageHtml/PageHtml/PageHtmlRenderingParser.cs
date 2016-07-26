// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Languages.Renderings;

namespace Sitecore.Pathfinder.PageHtml.PageHtml
{
    public class PageHtmlRenderingParser : RenderingParser
    {
        public PageHtmlRenderingParser() : base(".page.html", Constants.Templates.ViewRenderingId)
        {
        }

        protected override IEnumerable<string> GetPlaceholders(string contents)
        {
            return Enumerable.Empty<string>();
        }
    }
}
