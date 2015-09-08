// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Layouts;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Projects.Items.FieldResolvers.Layouts
{
    public abstract class LayoutResolverBase
    {
        private static readonly List<string> IgnoreAttributes = new List<string>
        {
            "Cacheable",
            "DataSource",
            "Placeholder",
            "RenderingName",
            "VaryByData",
            "VaryByDevice",
            "VaryByLogin",
            "VaryByParameters",
            "VaryByQueryString",
            "VaryByUser"
        };

        [NotNull]
        public virtual string Resolve([NotNull] LayoutResolveContext context, [NotNull] ITextNode textNode)
        {
            var writer = new StringWriter();
            var output = new XmlTextWriter(writer)
            {
                Formatting = Formatting.Indented
            };

            WriteLayout(context, output, textNode);

            var result = writer.ToString();
            return result;
        }

        [NotNull]
        protected virtual Item[] FindRenderingItems([NotNull] IEnumerable<Item> renderingItems, [NotNull] string renderingItemId)
        {
            var n = renderingItemId.LastIndexOf('.');
            if (n < 0)
            {
                return new Item[0];
            }

            renderingItemId = renderingItemId.Mid(n + 1);

            return renderingItems.Where(r => r.ShortName == renderingItemId).ToArray();
        }

        [NotNull]
        protected virtual string GetPlaceholders([NotNull] LayoutResolveContext context, [NotNull] ITextNode renderingTextNode, [NotNull] IProjectItem projectItem)
        {
            var item = projectItem as Item;
            if (item == null)
            {
                return string.Empty;
            }

            var id = renderingTextNode.GetAttributeValue("Id");
            var result = ",";

            var placeHoldersField = item.Fields.FirstOrDefault(f => string.Compare(f.FieldName, "Place Holders", StringComparison.OrdinalIgnoreCase) == 0);
            if (placeHoldersField != null)
            {
                foreach (var s in placeHoldersField.Value.Split(','))
                {
                    if (string.IsNullOrEmpty(s))
                    {
                        continue;
                    }

                    var placeholderName = s.Replace("$Id", id).Trim();

                    result += placeholderName + ",";
                }

                return result;
            }

            foreach (var placeholderName in AnalyzeFile(context, item, true))
            {
                result += placeholderName + ",";
            }

            return result;
        }

        protected virtual bool IsContentProperty([NotNull] ITextNode renderingTextNode, [NotNull] ITextNode childTextNode)
        {
            return childTextNode.Name.StartsWith(renderingTextNode.Name + ".");
        }

        [NotNull]
        protected virtual Item[] ResolveRenderingItem([NotNull] IEnumerable<Item> renderingItems, [NotNull] string renderingItemId)
        {
            var path = "/" + renderingItemId.Replace(".", "/");

            return renderingItems.Where(r => r.ItemIdOrPath.EndsWith(path, StringComparison.InvariantCultureIgnoreCase)).ToArray();
        }

        [NotNull]
        protected virtual Item[] ResolveRenderingItemId([NotNull] IEnumerable<Item> renderingItems, [NotNull] string renderingItemId)
        {
            var matches = renderingItems.Where(r => r.ShortName == renderingItemId).ToArray();

            if (matches.Length == 0)
            {
                matches = FindRenderingItems(renderingItems, renderingItemId);
            }

            if (matches.Length > 1)
            {
                matches = ResolveRenderingItem(matches, renderingItemId);
            }

            return matches;
        }

        protected virtual void WriteBool([NotNull] LayoutResolveContext context, [NotNull] XmlTextWriter output, [NotNull] ITextNode renderingTextNode, [NotNull] string id, [NotNull] string attributeName, [NotNull] string name, bool ignoreValue = false)
        {
            var value = renderingTextNode.GetAttributeValue(attributeName);
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (value != "True" && value != "False")
            {
                context.Field.WriteDiagnostic(Severity.Error, id + Texts.__Boolean_parameter_must_have_value__True__or__False_, renderingTextNode, attributeName);
                value = "False";
            }

            var b = string.Compare(value, "True", StringComparison.OrdinalIgnoreCase) == 0;
            if (b == ignoreValue)
            {
                return;
            }

            output.WriteAttributeString(name, b ? "1" : "0");
        }

        protected virtual void WriteDataSource([NotNull] LayoutResolveContext context, [NotNull] XmlTextWriter output, [NotNull] ITextNode renderingTextNode)
        {
            var dataSource = renderingTextNode.GetAttributeValue("DataSource");
            if (string.IsNullOrEmpty(dataSource))
            {
                return;
            }

            var item = context.Field.Item.Project.FindQualifiedItem(dataSource);
            if (item == null)
            {
                context.Field.WriteDiagnostic(Severity.Error, Texts.Datasource_not_found, dataSource);
                return;
            }

            output.WriteAttributeString("ds", item.Guid.Format());
        }

        protected virtual void WriteDevice([NotNull] LayoutResolveContext context, [NotNull] XmlTextWriter output, [NotNull] IEnumerable<Item> renderingItems, [NotNull] ITextNode deviceTextNode)
        {
            output.WriteStartElement("d");

            var deviceName = deviceTextNode.GetAttributeValue("Name");
            if (string.IsNullOrEmpty(deviceName))
            {
                context.Field.WriteDiagnostic(Severity.Error, Texts.Device_element_is_missing__Name__attribute_, deviceTextNode);
            }
            else
            {
                // todo: use proper template id or name
                var devices = context.Field.Item.Project.Items.OfType<Item>().Where(i => i.TemplateIdOrPath == "Device").ToList();
                if (!devices.Any())
                {
                    context.Field.WriteDiagnostic(Severity.Error, Texts.Device_item_not_found, deviceTextNode);
                }
                else
                {
                    var device = devices.FirstOrDefault(d => string.Compare(d.ItemName, deviceName, StringComparison.OrdinalIgnoreCase) == 0);
                    if (device == null)
                    {
                        context.Field.WriteDiagnostic(Severity.Error, Texts.Device_not_found, deviceTextNode, deviceName);
                    }
                    else
                    {
                        output.WriteAttributeString("id", device.Guid.Format());
                    }
                }
            }

            var layoutPlaceholders = string.Empty;
            var layoutPath = deviceTextNode.GetAttributeTextNode("Layout");
            if (layoutPath != null && !string.IsNullOrEmpty(layoutPath.Value))
            {
                var l = context.Field.Item.Project.FindQualifiedItem(layoutPath.Value);
                if (l == null)
                {
                    context.Field.WriteDiagnostic(Severity.Error, Texts.Layout_not_found_, layoutPath.Value);
                    return;
                }

                output.WriteAttributeString("l", l.Guid.Format());
                layoutPlaceholders = GetPlaceholders(context, deviceTextNode, l);
            }

            var renderings = context.Snapshot.GetJsonChildTextNode(deviceTextNode, "Renderings");
            if (renderings == null)
            {
                // silent
                return;
            }

            foreach (var renderingTextNode in renderings.ChildNodes)
            {
                WriteRendering(context, output, renderingItems, renderingTextNode, layoutPlaceholders);
            }

            output.WriteEndElement();
        }

        protected virtual void WriteLayout([NotNull] LayoutResolveContext context, [NotNull] XmlTextWriter output, [NotNull] ITextNode layoutTextNode)
        {
            // todo: cache this in the build context
            // todo: use better search
            // todo: add renderings from project
            var renderingItems = context.Field.Item.Project.Items.OfType<Rendering>().Select(r => r.Item).ToList();

            output.WriteStartElement("r");

            var devices = context.Snapshot.GetJsonChildTextNode(layoutTextNode, "Devices");
            if (devices == null)
            {
                // silent
                return;
            }

            foreach (var deviceTextNode in devices.ChildNodes)
            {
                WriteDevice(context, output, renderingItems, deviceTextNode);
            }

            output.WriteEndElement();
        }

        protected virtual void WritePlaceholder([NotNull] LayoutResolveContext context, [NotNull] XmlTextWriter output, [NotNull] ITextNode renderingTextNode, [NotNull] string id, [NotNull] string placeholders)
        {
            var placeholder = renderingTextNode.GetAttributeValue("Placeholder");

            if (string.IsNullOrEmpty(placeholder) && !string.IsNullOrEmpty(placeholders))
            {
                var n = placeholders.IndexOf(",", 1, StringComparison.InvariantCultureIgnoreCase);
                if (n >= 0)
                {
                    placeholder = placeholders.Mid(1, n - 1);
                }
            }

            if (string.IsNullOrEmpty(placeholder))
            {
                return;
            }

            if (!string.IsNullOrEmpty(placeholders))
            {
                if (placeholders.IndexOf("," + placeholder + ",", StringComparison.InvariantCultureIgnoreCase) < 0)
                {
                    context.Field.WriteDiagnostic(Severity.Warning, string.Format(Texts._2___Placeholder___0___is_not_defined_in_the_parent_rendering__Parent_rendering_has_these_placeholders___1__, placeholder, placeholders.Mid(1, placeholders.Length - 2), id), renderingTextNode, "Placeholder");
                }
            }

            output.WriteAttributeString("ph", placeholder);
        }

        protected virtual void WriteRendering([NotNull] LayoutResolveContext context, [NotNull] XmlTextWriter output, [NotNull] IEnumerable<Item> renderingItems, [NotNull] ITextNode renderingTextNode, [NotNull] string placeholders)
        {
            string renderingItemId;

            if (renderingTextNode.Name == "r")
            {
                renderingItemId = renderingTextNode.GetAttributeValue("id");
            }
            else if (renderingTextNode.Name == "Rendering")
            {
                renderingItemId = renderingTextNode.GetAttributeValue("RenderingName");
            }
            else
            {
                renderingItemId = renderingTextNode.Name;
            }

            var id = renderingTextNode.GetAttributeValue("Id");
            if (string.IsNullOrEmpty(id))
            {
                id = renderingItemId;
            }

            if (string.IsNullOrEmpty(renderingItemId))
            {
                context.Field.WriteDiagnostic(Severity.Error, $"Unknown element \"{id}\".", renderingTextNode);
                return;
            }

            Item renderingItem;
            if (renderingItemId.IsGuid())
            {
                renderingItem = context.Field.Item.Project.Items.OfType<Item>().FirstOrDefault(i => i.Guid.Format() == renderingItemId);
            }
            else
            {
                var matches = ResolveRenderingItemId(renderingItems, renderingItemId);

                if (matches.Length == 0)
                {
                    context.Field.WriteDiagnostic(Severity.Error, $"Rendering \"{renderingItemId}\" not found.", renderingTextNode);
                    return;
                }

                if (matches.Length > 1)
                {
                    context.Field.WriteDiagnostic(Severity.Error, $"Ambiguous rendering match. {matches.Length} renderings match \"{renderingItemId}\".", renderingTextNode);
                    return;
                }

                renderingItem = matches[0];
            }

            if (renderingItem == null)
            {
                context.Field.WriteDiagnostic(Severity.Error, $"Rendering \"{renderingItemId}\" not found.", renderingTextNode);
                return;
            }

            output.WriteStartElement("r");

            WriteBool(context, output, renderingTextNode, id, "Cacheable", "cac");
            output.WriteAttributeString("id", renderingItem.Guid.Format());
            WriteDataSource(context, output, renderingTextNode);
            WriteParameters(context, output, renderingTextNode, renderingItem, id);
            WritePlaceholder(context, output, renderingTextNode, id, placeholders);

            // WriteAttributeStringNotEmpty(@"uid", this.UniqueId);
            WriteBool(context, output, renderingTextNode, id, "VaryByData", "vbd");
            WriteBool(context, output, renderingTextNode, id, "VaryByDevice", "vbdev");
            WriteBool(context, output, renderingTextNode, id, "VaryByLogin", "vbl");
            WriteBool(context, output, renderingTextNode, id, "VaryByParameters", "vbp");
            WriteBool(context, output, renderingTextNode, id, "VaryByQueryString", "vbqs");
            WriteBool(context, output, renderingTextNode, id, "VaryByUser", "vbu");

            output.WriteEndElement();

            /*
            if (renderingTextNode.ChildNodes.Any(child => !IsContentProperty(renderingTextNode, child)))
            {
                var placeHolders = renderingItem["Place Holders"];

                if (string.IsNullOrEmpty(placeHolders))
                {
                    context.Trace.TraceError($"The \"{renderingTextNode.Name}\" element cannot have any child elements as it does not define any placeholders in its 'Place Holders' field.", renderingTextNode);
                }
                else if (placeHolders.IndexOf("$Id", StringComparison.InvariantCulture) >= 0 && string.IsNullOrEmpty(renderingTextNode.GetAttributeValue("Id")))
                {
                    context.Trace.TraceError($"The \"{renderingTextNode.Name}\" element must have an ID as it has child elements.", renderingTextNode);
                }
            }
            */

            foreach (var child in renderingTextNode.ChildNodes)
            {
                if (IsContentProperty(renderingTextNode, child))
                {
                    continue;
                }

                WriteRendering(context, output, renderingItems, child, GetPlaceholders(context, renderingTextNode, renderingItem));
            }
        }

        private IEnumerable<string> AnalyzeFile([NotNull] LayoutResolveContext context, Item item, bool includeDynamicPlaceholders)
        {
            var pathField = item.Fields.FirstOrDefault(f => string.Compare(f.FieldName, "Path", StringComparison.OrdinalIgnoreCase) == 0);
            if (pathField == null)
            {
                return Enumerable.Empty<string>();
            }

            var path = PathHelper.Combine(item.Project.Options.ProjectDirectory, pathField.Value.TrimStart('/'));
            if (!context.FileSystem.FileExists(path))
            {
                return Enumerable.Empty<string>();
            }

            var source = context.FileSystem.ReadAllText(path);

            if (string.Compare(Path.GetExtension(path), ".ascx", StringComparison.CurrentCultureIgnoreCase) == 0 || string.Compare(Path.GetExtension(path), ".aspx", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                return AnalyzeWebFormsFile(source);
            }

            if (string.Compare(Path.GetExtension(path), ".cshtml", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                return includeDynamicPlaceholders ? AnalyzeViewFileWithDynamicPlaceholders(source) : AnalyzeViewFile(source);
            }

            return Enumerable.Empty<string>();
        }

        private IEnumerable<string> AnalyzeViewFile(string source)
        {
            var matches = Regex.Matches(source, "\\@Html\\.Sitecore\\(\\)\\.Placeholder\\(\"([^\"]*)\"\\)", RegexOptions.IgnoreCase);
            return matches.OfType<Match>().Select(i => i.Groups[1].ToString().Trim());
        }

        private IEnumerable<string> AnalyzeViewFileWithDynamicPlaceholders(string source)
        {
            var matches = Regex.Matches(source, "\\@Html\\.Sitecore\\(\\)\\.Placeholder\\(([^\"\\)]*)\"([^\"]*)\"\\)", RegexOptions.IgnoreCase);

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

        private IEnumerable<string> AnalyzeWebFormsFile(string source)
        {
            var matches = Regex.Matches(source, "<[^>]*Placeholder[^>]*Key=\"([^\"]*)\"[^>]*>", RegexOptions.IgnoreCase);

            return matches.OfType<Match>().Select(i => i.Groups[1].ToString().Trim());
        }

        private void WriteParameters([NotNull] LayoutResolveContext context, [NotNull] XmlTextWriter output, [NotNull] ITextNode renderingTextNode, [NotNull] Item renderingItem, [NotNull] string id)
        {
            var fields = new Dictionary<string, string>();

            var parametersTemplateItemId = renderingItem.Fields.FirstOrDefault(f => f.FieldName == "Parameters Template")?.Value ?? string.Empty;
            var parametersTemplateItem = context.Field.Item.Project.FindQualifiedItem(parametersTemplateItemId) as Template;
            if (parametersTemplateItem != null)
            {
                foreach (var field in parametersTemplateItem.Sections.SelectMany(s => s.Fields))
                {
                    fields[field.FieldName.ToLowerInvariant()] = field.Type;
                }
            }

            var properties = new Dictionary<string, string>();

            foreach (var attribute in renderingTextNode.Attributes)
            {
                properties[attribute.Name] = attribute.Value;
            }

            foreach (var child in renderingTextNode.ChildNodes)
            {
                if (IsContentProperty(renderingTextNode, child))
                {
                    var name = child.Name.Mid(renderingTextNode.Name.Length + 1);
                    var value = string.Join(string.Empty, child.ChildNodes.Select(n => n.ToString()).ToArray());

                    properties[name] = value;
                }
            }

            var par = new UrlString();
            foreach (var pair in properties)
            {
                var attributeName = pair.Key;
                if (IgnoreAttributes.Contains(attributeName))
                {
                    continue;
                }

                var value = pair.Value;

                string type;
                if (fields.TryGetValue(attributeName.ToLowerInvariant(), out type))
                {
                    switch (type.ToLowerInvariant())
                    {
                        case "checkbox":
                            if (!value.StartsWith("{Binding") && !value.StartsWith("{@"))
                            {
                                if (value != "True" && value != "False")
                                {
                                    context.Field.WriteDiagnostic(Severity.Error, $"{id}: Boolean parameter must have value \"True\", \"False\", \"{{Binding ... }}\" or \"{{@ ... }}\".", renderingTextNode, attributeName);
                                }

                                value = value == "1" || string.Compare(value, "true", StringComparison.OrdinalIgnoreCase) == 0 ? "1" : "0";
                            }

                            break;
                    }
                }
                else
                {
                    context.Field.WriteDiagnostic(Severity.Warning, string.Format(Texts._1___Parameter___0___is_not_defined_in_the_parameters_template_, attributeName, id), renderingTextNode, attributeName);
                }

                if (value.StartsWith("/sitecore", StringComparison.InvariantCultureIgnoreCase))
                {
                    var item = context.Field.Item.Project.FindQualifiedItem(value);
                    if (item != null)
                    {
                        value = item.Guid.Format();
                    }
                }

                par[attributeName] = value;
            }

            output.WriteAttributeString("par", par.ToString());
        }
    }
}
