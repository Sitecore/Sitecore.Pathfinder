// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Languages.Renderings;

namespace Sitecore.Pathfinder.React
{
    public class JsxRenderingParser : RenderingParser
    {
        public JsxRenderingParser() : base(".jsx", Constants.Templates.ViewRendering)
        {                       
        }

        protected override IEnumerable<string> GetPlaceholders(string contents)
        {
            return Enumerable.Empty<string>();
        }
    }
}
