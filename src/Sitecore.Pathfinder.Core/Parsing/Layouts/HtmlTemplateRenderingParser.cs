// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sitecore.Pathfinder.Parsing.Layouts
{
    public class HtmlTemplateRenderingParser : RenderingParser
    {
        public HtmlTemplateRenderingParser() : base(".html", Constants.Templates.ViewRendering)
        {
        }

        protected override IEnumerable<string> GetPlaceholders(string contents)
        {
            var matches = Regex.Matches(contents, "\\{\\{\\%([^\\}]*)\\}\\}", RegexOptions.IgnoreCase);

            return matches.OfType<Match>().Select(i => i.Groups[1].ToString().Trim());
        }
    }
}
