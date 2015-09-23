// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Parsing.Layouts
{
    [Export(typeof(IParser))]
    public class ViewRenderingParser : RenderingParser
    {
        public ViewRenderingParser() : base(".cshtml", Constants.Templates.ViewRendering)
        {
        }

        protected override IEnumerable<string> GetPlaceholders(string contents)
        {
            var matches = Regex.Matches(contents, "\\@Html\\.Sitecore\\(\\)\\.Placeholder\\(([^\"\\)]*)\"([^\"]*)\"\\)", RegexOptions.IgnoreCase);

            var result = new List<string>();
            foreach (var match in matches.OfType<Match>())
            {
                var prefix = match.Groups[1].ToString().Trim();
                var name = match.Groups[2].ToString().Trim();

                if (!string.IsNullOrEmpty(prefix))
                {
                    if (name.StartsWith("."))
                    {
                        name = name.Mid(1);
                    }

                    name = "$Id." + name;
                }

                result.Add(name);
            }

            return result;
        }
    }
}
