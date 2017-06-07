// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Languages.Renderings
{
    public abstract class WebFormsRenderingParser : RenderingParser
    {
        [NotNull]
        private static readonly Regex PlaceholderRegex = new Regex("<[^>]*Placeholder[^>]*Key=\"([^\"]*)\"[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        protected WebFormsRenderingParser([NotNull] IConfiguration configuration, [NotNull] string fileExtension, [NotNull] string templateIdOrPath) : base(configuration, fileExtension, templateIdOrPath)
        {
        }

        protected override IEnumerable<string> GetPlaceholders(string contents)
        {
            var matches = PlaceholderRegex.Matches(contents);

            return matches.OfType<Match>().Select(i => i.Groups[1].ToString().Trim());
        }
    }
}
