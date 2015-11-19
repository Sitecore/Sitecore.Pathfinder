// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Languages.Renderings;

namespace Sitecore.Pathfinder.Languages.HtmlTemplates
{
    public class HtmlTemplateRenderingParser : RenderingParser
    {
        [NotNull]
        private static readonly Regex regex = new Regex("\\{\\{\\%([^\\}]*)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public HtmlTemplateRenderingParser() : base(".html", Constants.Templates.ViewRendering)
        {
        }

        protected override IEnumerable<string> GetPlaceholders(string contents)
        {
            var matches = regex.Matches(contents);

            return matches.OfType<Match>().Select(i => i.Groups[1].ToString().Trim());
        }
    }
}
