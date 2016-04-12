// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sitecore.Pathfinder.Languages.Renderings;

namespace Sitecore.Pathfinder.HtmlFile.HtmlFiles
{
    public class HtmlFileRenderingParser : RenderingParser
    {
        [NotNull]
        private static readonly Regex PlaceholderRegex = new Regex("\\{\\{\\%([^\\}]*)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public HtmlFileRenderingParser() : base(".html", Constants.Templates.ViewRendering)
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