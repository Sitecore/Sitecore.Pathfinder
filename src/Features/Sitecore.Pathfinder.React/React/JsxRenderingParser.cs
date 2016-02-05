// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sitecore.Pathfinder.Languages.Renderings;

namespace Sitecore.Pathfinder.React.React
{
    public class JsxRenderingParser : RenderingParser
    {
        private static readonly Regex PlaceholderRegex = new Regex("\\{[\\s]*this\\.props\\.placeholders\\.([^\\}]*)[\\s]*\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public JsxRenderingParser() : base(".jsx", Constants.Templates.ViewRendering)
        {                       
        }

        protected override IEnumerable<string> GetPlaceholders(string contents)
        {
            var matches = PlaceholderRegex.Matches(contents);

            var placeholders = matches.OfType<Match>().Select(i => i.Groups[1].ToString().Trim());
            return placeholders;
        }
    }
}
