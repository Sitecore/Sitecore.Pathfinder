// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Text.RegularExpressions;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Extensions.StringExtensions;
using Sitecore.IO;
using Sitecore.Links;
using Sitecore.Mvc;
using Sitecore.Mvc.Helpers;
using Sitecore.Mvc.Presentation;
using Sitecore.Pathfinder.IO;
using Sitecore.Resources;
using Sitecore.Web.UI;
using Sitecore.Web.UI.HtmlControls.Data;

namespace Sitecore.Pathfinder.Mvc.Presentation
{
    public class HtmlTemplateRenderer : Renderer
    {
        private static readonly Regex Contexts = new Regex("\\{\\{=([^\\}]*)\\}\\}([\\S\\W]*)\\{\\{/\\1\\}\\}", RegexOptions.Compiled);

        private static readonly Regex Inverted = new Regex("\\{\\{\\^([^\\}]*)\\}\\}([\\S\\W]*)\\{\\{/\\1\\}\\}", RegexOptions.Compiled);

        private static readonly Regex Lists = new Regex("\\{\\{#([^\\}]*)\\}\\}([\\S\\W]*)\\{\\{/\\1\\}\\}", RegexOptions.Compiled);

        private static readonly Regex Mustaches = new Regex("\\{\\{([^\\}]*)\\}\\}", RegexOptions.Compiled);

        [Diagnostics.NotNull]
        public string FilePath { get; set; } = string.Empty;

        [Diagnostics.CanBeNull]
        public Rendering Rendering { get; set; }

        public override void Render([Diagnostics.NotNull] TextWriter writer)
        {
            var sitecoreHelper = PageContext.Current.HtmlHelper.Sitecore();

            var output = Render(sitecoreHelper, Context.Item, FilePath);

            writer.Write(output);
        }

        [Diagnostics.NotNull]
        private string Process([Diagnostics.NotNull] SitecoreHelper sitecoreHelper, [Diagnostics.NotNull] Item contextItem, [Diagnostics.NotNull] string output)
        {
            output = ProcessLists(sitecoreHelper, contextItem, output);
            output = ProcessContexts(sitecoreHelper, contextItem, output);
            output = ProcessInverted(sitecoreHelper, contextItem, output);
            output = ProcessValues(sitecoreHelper, contextItem, output);

            return output;
        }

        [Diagnostics.NotNull]
        private string ProcessContexts([Diagnostics.NotNull] SitecoreHelper sitecoreHelper, [Diagnostics.NotNull] Item contextItem, [Diagnostics.NotNull] string output)
        {
            MatchEvaluator evaluator = delegate(Match match)
            {
                var expression = match.Groups[1].Value.Trim();
                var text = match.Groups[2].Value.Trim();

                var item = contextItem.Database.GetItem(expression);
                return item == null ? string.Empty : Process(sitecoreHelper, item, text);
            };

            return Contexts.Replace(output, evaluator);
        }

        [Diagnostics.NotNull]
        private string ProcessInverted([Diagnostics.NotNull] SitecoreHelper sitecoreHelper, [Diagnostics.NotNull] Item contextItem, [Diagnostics.NotNull] string output)
        {
            MatchEvaluator evaluator = delegate(Match match)
            {
                var expression = match.Groups[1].Value.Trim();
                var text = match.Groups[2].Value.Trim();

                var value = contextItem[expression];
                var isVisible = !string.IsNullOrEmpty(value) && value != "0" && string.Compare(value, "false", StringComparison.OrdinalIgnoreCase) != 0;
                return !isVisible ? Process(sitecoreHelper, contextItem, text) : string.Empty;
            };

            return Inverted.Replace(output, evaluator);
        }

        [Diagnostics.NotNull]
        private string ProcessLists([Diagnostics.NotNull] SitecoreHelper sitecoreHelper, [Diagnostics.NotNull] Item contextItem, [Diagnostics.NotNull] string output)
        {
            MatchEvaluator evaluator = delegate(Match match)
            {
                var expression = match.Groups[1].Value.Trim();
                var text = match.Groups[2].Value.Trim();

                if (expression.IndexOf('/') >= 0 || expression.IndexOf('.') >= 0)
                {
                    var items = LookupSources.GetItems(contextItem, expression);
                    var list = string.Empty;

                    foreach (var newContextitem in items)
                    {
                        list += Process(sitecoreHelper, newContextitem, text);
                    }

                    return list;
                }

                var value = contextItem[expression];
                var isVisible = !string.IsNullOrEmpty(value) && value != "0" && string.Compare(value, "false", StringComparison.OrdinalIgnoreCase) != 0;
                return isVisible ? Process(sitecoreHelper, contextItem, text) : string.Empty;
            };

            return Lists.Replace(output, evaluator);
        }

        [Diagnostics.NotNull]
        private string ProcessValues([Diagnostics.NotNull] SitecoreHelper sitecoreHelper, [Diagnostics.NotNull] Item contextItem, [Diagnostics.NotNull] string output)
        {
            MatchEvaluator evaluator = delegate(Match match)
            {
                var text = match.Groups[1].Value.Trim();

                if (text.StartsWith("!", StringComparison.Ordinal))
                {
                    // comments are ignored
                    return string.Empty;
                }

                if (text.StartsWith("%", StringComparison.Ordinal))
                {
                    // placeholders - extension to Mustache
                    var placeHolderName = text.Mid(1).Trim();
                    return sitecoreHelper.Placeholder(placeHolderName).ToString();
                }

                if (text.StartsWith(">", StringComparison.Ordinal))
                {
                    // partials
                    var filePath = text.Mid(1).Trim();

                    var directory = Path.GetDirectoryName(FileUtil.MapPath(FilePath)) ?? string.Empty;
                    filePath = PathHelper.Combine(directory, filePath);

                    return sitecoreHelper.ViewRendering(filePath).ToString();
                }

                switch (text.ToLowerInvariant())
                {
                    case "@id":
                        return contextItem.ID.ToString();
                    case "@name":
                        return contextItem.Name;
                    case "@displayname":
                        return contextItem.Appearance.DisplayName;
                    case "@path":
                        return contextItem.Paths.Path;
                    case "@templatename":
                        return contextItem.TemplateName;
                    case "@templateid":
                        return contextItem.TemplateID.ToString();
                    case "@url":
                        return LinkManager.GetItemUrl(contextItem);
                    case "@icon16x16":
                        return Images.GetImage(contextItem.Appearance.Icon, ImageDimension.id16x16, "center");
                    case "@icon24x24":
                        return Images.GetImage(contextItem.Appearance.Icon, ImageDimension.id24x24, "center");
                    case "@icon32x32":
                        return Images.GetImage(contextItem.Appearance.Icon, ImageDimension.id32x32, "center");
                    case "@icon48x48":
                        return Images.GetImage(contextItem.Appearance.Icon, ImageDimension.id48x48, "center");
                }

                // search up the tree for the field - this is specified by Mustache
                var item = contextItem;
                while (item != null)
                {
                    var template = TemplateManager.GetTemplate(item);
                    if (template?.GetField(text) != null)
                    {
                        break;
                    }

                    item = item.Parent;
                }

                // todo: Html escaping is not working
                return item == null ? string.Empty : sitecoreHelper.Field(text, item).ToString();
            };

            return Mustaches.Replace(output, evaluator);
        }

        [Diagnostics.NotNull]
        private string Render([Diagnostics.NotNull] SitecoreHelper sitecoreHelper, [Diagnostics.NotNull] Item contextItem, [Diagnostics.NotNull] string filePath)
        {
            if (!FileUtil.FileExists(filePath))
            {
                return string.Empty;
            }

            var output = FileUtil.ReadFromFile(filePath);

            output = Process(sitecoreHelper, contextItem, output);

            return output;
        }
    }
}
