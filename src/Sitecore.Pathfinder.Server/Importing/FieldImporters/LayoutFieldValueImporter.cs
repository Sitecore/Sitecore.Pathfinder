// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Languages;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Importing
{
    public class LayoutFieldValueImporter : FieldValueImporterBase
    {
        [Diagnostics.NotNull]
        private static readonly Regex SafeName = new Regex("^[a-zA-Z0-9_\\-\\.]+$", RegexOptions.Compiled);

        public override bool CanImport(Field field, Item item, ILanguage language, string value)
        {
            return field.Name == "__Renderings" || string.Equals(field.Type, "Layout", StringComparison.OrdinalIgnoreCase);
        }

        public override string Import(Field field, Item item, ILanguage language, string value)
        {
            var layoutBuilder = new LayoutBuilder();

            BuildLayout(layoutBuilder, item, value);

            var stringWriter = new StringWriter();

            language.WriteLayout(stringWriter, item.Database.Name, layoutBuilder);

            return stringWriter.ToString();
        }

        protected virtual void BuildDevice([Diagnostics.NotNull] DeviceBuilder deviceBuilder, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull] Item deviceItem, [Diagnostics.NotNull] XElement deviceElement, [Diagnostics.NotNull] [ItemNotNull] List<Item> renderingItems)
        {
            deviceBuilder.DeviceName = deviceItem.Name;

            var layoutItem = item.Database.GetItem(deviceElement.GetAttributeValue("l"));
            if (layoutItem != null)
            {
                deviceBuilder.LayoutItemPath = layoutItem.Paths.Path;
            }

            foreach (var renderingElement in deviceElement.Elements())
            {
                var renderingItem = item.Database.GetItem(renderingElement.GetAttributeValue("id"));
                if (renderingItem == null)
                {
                    continue;
                }

                var renderingModel = new RenderingBuilder(deviceBuilder);

                BuildRendering(renderingModel, renderingItem, renderingElement, renderingItems);

                deviceBuilder.Renderings.Add(renderingModel);
            }

            foreach (var rendering in deviceBuilder.Renderings)
            {
                rendering.ParentRendering = deviceBuilder.Renderings.FirstOrDefault(r => r.Placeholders.Contains(rendering.Placeholder, StringComparer.OrdinalIgnoreCase));
                if (rendering.ParentRendering == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(rendering.Placeholder))
                {
                    continue;
                }

                // if the renderings placeholder is the parents first placeholder, omit the rendering placeholder since it is the default
                if (string.Equals(rendering.ParentRendering.Placeholders.FirstOrDefault(), rendering.Placeholder, StringComparison.OrdinalIgnoreCase))
                {
                    rendering.Placeholder = string.Empty;
                }
            }
        }

        protected virtual void BuildLayout([Diagnostics.NotNull] LayoutBuilder layoutBuilder, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull] string layout)
        {
            if (string.IsNullOrEmpty(layout))
            {
                return;
            }

            var layoutElement = layout.ToXElement();
            if (layoutElement == null)
            {
                return;
            }

            var renderingItems = item.Database.GetItemsByTemplate(ServerConstants.Renderings.ViewRenderingId, TemplateIDs.XSLRendering, TemplateIDs.Sublayout, ServerConstants.Renderings.WebcontrolRendering, ServerConstants.Renderings.UrlRendering, ServerConstants.Renderings.MethodRendering).GroupBy(i => i.Name).Select(i => i.First()).OrderBy(i => i.Name).ToList();

            foreach (var deviceElement in layoutElement.Elements())
            {
                var deviceId = deviceElement.GetAttributeValue("id");
                if (string.IsNullOrEmpty(deviceId))
                {
                    continue;
                }

                var deviceItem = item.Database.GetItem(deviceId);
                if (deviceItem == null)
                {
                    continue;
                }

                var deviceModel = new DeviceBuilder(layoutBuilder);

                BuildDevice(deviceModel, item, deviceItem, deviceElement, renderingItems);

                layoutBuilder.Devices.Add(deviceModel);
            }
        }

        protected virtual void BuildRendering([Diagnostics.NotNull] RenderingBuilder renderingBuilder, [Diagnostics.NotNull] Item renderingItem, [Diagnostics.NotNull] XElement renderingElement, [Diagnostics.NotNull] [ItemNotNull] List<Item> renderingItems)
        {
            var parameters = new UrlString(renderingElement.GetAttributeValue("par"));
            renderingBuilder.Id = parameters.Parameters["Id"];
            renderingBuilder.Placeholder = renderingElement.GetAttributeValue("ph");

            foreach (var placeholder in renderingItem["Place Holders"].Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries))
            {
                renderingBuilder.Placeholders.Add(placeholder.Replace("$Id", renderingBuilder.Id).Trim());
            }

            var fields = new Dictionary<string, TemplateField>();

            var parametersTemplateItem = renderingItem.Database.GetItem(renderingItem["Parameters Template"]);
            if (parametersTemplateItem != null)
            {
                var template = TemplateManager.GetTemplate(parametersTemplateItem.ID, renderingItem.Database);
                if (template != null)
                {
                    foreach (var field in template.GetFields(true))
                    {
                        if (field.Template.BaseIDs.Length != 0)
                        {
                            fields[field.Name.ToLowerInvariant()] = field;
                        }
                    }
                }
            }

            // rendering name
            var name = renderingItem.Name;

            var duplicates = renderingItems.Where(i => i.Name == renderingItem.Name).ToList();
            if (duplicates.Count > 1)
            {
                name = GetUniqueRenderingName(duplicates, renderingItem);
            }

            renderingBuilder.UnsafeName = !SafeName.IsMatch(name);
            renderingBuilder.Name = name;

            // parameters
            foreach (var key in parameters.Parameters.Keys.OfType<string>().Where(k => !string.IsNullOrEmpty(k)).OrderBy(k => k))
            {
                var value = parameters.Parameters[key];
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                value = HttpUtility.UrlDecode(value) ?? string.Empty;

                TemplateField field;
                if (fields.TryGetValue(key.ToLowerInvariant(), out field))
                {
                    switch (field.Type.ToLowerInvariant())
                    {
                        case "checkbox":
                            if (!value.StartsWith("{Binding"))
                            {
                                value = MainUtil.GetBool(value, false) ? "True" : "False";
                            }

                            break;

                        case "droptree":
                            if (ID.IsID(value))
                            {
                                var i = renderingItem.Database.GetItem(value);
                                if (i != null)
                                {
                                    value = i.Paths.Path;
                                }
                            }

                            break;
                    }

                    var source = new Sitecore.Text.UrlString(field.Source);
                    var defaultValue = source.Parameters["defaultvalue"] ?? string.Empty;

                    if (string.Equals(value, defaultValue, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }

                // todo: Hacky, hacky, hacky
                if ((key == "IsEnabled" || key == "IsVisible") && value == "True")
                {
                    continue;
                }

                renderingBuilder.Attributes[key] = value;
            }

            // data source
            var dataSource = renderingElement.GetAttributeValue("ds");
            if (ID.IsID(dataSource))
            {
                var dataSourceItem = renderingItem.Database.GetItem(dataSource);
                if (dataSourceItem != null)
                {
                    dataSource = dataSourceItem.Paths.Path;
                }
            }

            renderingBuilder.DataSource = dataSource;

            // caching
            renderingBuilder.Cacheable = renderingElement.GetAttributeValue("cac") == @"1";
            renderingBuilder.VaryByData = renderingElement.GetAttributeValue("vbd") == @"1";
            renderingBuilder.VaryByDevice = renderingElement.GetAttributeValue("vbdev") == @"1";
            renderingBuilder.VaryByLogin = renderingElement.GetAttributeValue("vbl") == @"1";
            renderingBuilder.VaryByParameters = renderingElement.GetAttributeValue("vbp") == @"1";
            renderingBuilder.VaryByQueryString = renderingElement.GetAttributeValue("vbqs") == @"1";
            renderingBuilder.VaryByUser = renderingElement.GetAttributeValue("vbu") == @"1";
        }

        [Diagnostics.NotNull]
        protected virtual string GetUniqueRenderingName([NotNull] [ItemNotNull] IEnumerable<Item> duplicates, [Diagnostics.NotNull] Item renderingItem)
        {
            var paths = duplicates.Where(r => r.Name == renderingItem.Name && r.ID != renderingItem.ID).Select(r => r.Paths.Path).ToList();
            var parts = renderingItem.Paths.Path.Split(Constants.Slash, StringSplitOptions.RemoveEmptyEntries);

            var result = string.Empty;

            for (var i = parts.Length - 1; i >= 0; i--)
            {
                result = "/" + parts[i] + result;

                var r = result;
                if (!paths.Any(p => p.EndsWith(r, StringComparison.InvariantCultureIgnoreCase)))
                {
                    break;
                }
            }

            result = result.Mid(1).Replace("/", ".");

            return result;
        }
    }
}
