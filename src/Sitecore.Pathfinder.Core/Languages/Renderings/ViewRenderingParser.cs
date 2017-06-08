// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Languages.Renderings
{
    [Export(typeof(IParser)), Shared]
    public class ViewRenderingParser : RenderingParser
    {
        [NotNull]
        private static readonly Regex PlaceholderRegex = new Regex("\\@Html\\.Sitecore\\(\\)\\.Placeholder\\(([^\"\\)]*)\"([^\"]*)\"\\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        [ImportingConstructor]
        public ViewRenderingParser([NotNull] IConfiguration configuration, [NotNull] IFactory factory) : base(configuration, factory, ".cshtml", Constants.Templates.ViewRenderingId)
        {
        }

        protected override IEnumerable<string> GetPlaceholders(string contents)
        {
            var matches = PlaceholderRegex.Matches(contents);

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
