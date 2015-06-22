// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Text.RegularExpressions;
using Sitecore.Extensions.StringExtensions;
using Sitecore.IO;
using Sitecore.Mvc;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Pathfinder.Mvc.Presentation
{
    public class MustacheHtmlRenderer : Renderer
    {
        [Diagnostics.NotNull]
        public string FilePath { get; set; } = string.Empty;

        [Diagnostics.CanBeNull]
        public Rendering Rendering { get; set; }

        public override void Render([Diagnostics.NotNull] TextWriter writer)
        {
            var output = FileUtil.ReadFromFile(FilePath);

            var sitecoreHelper = PageContext.Current.HtmlHelper.Sitecore();

            MatchEvaluator mustacheEvaluator = delegate(Match match)
            {
                var text = match.Groups[1].Value.Trim();

                if (text.StartsWith("placeholder ", StringComparison.InvariantCultureIgnoreCase))
                {
                    var placeHolderName = text.Mid(12).Trim();
                    return sitecoreHelper.Placeholder(placeHolderName).ToString();
                }

                if (text.StartsWith(">", StringComparison.InvariantCultureIgnoreCase))
                {
                    var placeHolderName = text.Mid(1).Trim();
                    return sitecoreHelper.Placeholder(placeHolderName).ToString();
                }

                return sitecoreHelper.Field(text).ToString();
            };

            var result = Regex.Replace(output, "\\{\\{([^\\}]*)\\}\\}", mustacheEvaluator);

            writer.Write(result);
        }
    }
}
