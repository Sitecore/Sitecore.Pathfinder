// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Languages.Renderings
{
    public abstract class WebFormsRenderingParser : RenderingParser
    {
        protected WebFormsRenderingParser([NotNull] string fileExtension, [NotNull] string templateIdOrPath) : base(fileExtension, templateIdOrPath)
        {
        }

        protected override IEnumerable<string> GetPlaceholders(string contents)
        {
            var matches = Regex.Matches(contents, "<[^>]*Placeholder[^>]*Key=\"([^\"]*)\"[^>]*>", RegexOptions.IgnoreCase);

            return matches.OfType<Match>().Select(i => i.Groups[1].ToString().Trim());
        }
    }
}
