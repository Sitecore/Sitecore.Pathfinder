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
using Sitecore.Pathfinder.Languages.Renderings;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    public class LayoutCompiler
    {
        [NotNull, ItemNotNull]
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
        public virtual string Compile([NotNull] LayoutCompileContext context, [NotNull] ITextNode textNode)
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

        [NotNull, ItemNotNull]
        protected virtual Item[] FindRenderingItems([NotNull, ItemNotNull] IEnumerable<Item> renderingItems, [NotNull] string renderingItemId)
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
        protected virtual string GetPlaceholders([NotNull] LayoutCompileContext context, [NotNull] ITextNode renderingTextNode, [NotNull] IProjectItem projectItem)
        {
            var item = projectItem as Item;
            if (item == null)
            {
                return string.Empty;
            }

            var id = renderingTextNode.GetAttributeValue("Id");
            var result = ",";

            var placeHoldersField = item.Fields.FirstOrDefault(f => string.Equals(f.FieldName, "Place Holders", StringComparison.OrdinalIgnoreCase));
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
            return childTextNode.Key.StartsWith(renderingTextNode.Key + ".");
        }

        [NotNull, ItemNotNull]
        protected virtual Item[] ResolveRenderingItem([NotNull, ItemNotNull] IEnumerable<Item> renderingItems, [NotNull] string renderingItemId)
        {
            var path = "/" + renderingItemId.Replace(".", "/");

            return renderingItems.Where(r => r.ItemIdOrPath.EndsWith(path, StringComparison.InvariantCultureIgnoreCase)).ToArray();
        }

        [NotNull, ItemNotNull]
        protected virtual Item[] ResolveRenderingItemId([NotNull, ItemNotNull] IEnumerable<Item> renderingItems, [NotNull] string renderingItemId)
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

        protected virtual void WriteBool([NotNull] LayoutCompileContext context, [NotNull] XmlTextWriter output, [NotNull] ITextNode renderingTextNode, [NotNull] string id, [NotNull] string attributeName, [NotNull] string name, bool ignoreValue = false)
        {
            var value = renderingTextNode.GetAttributeValue(attributeName);
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (value != "True" && value != "False")
            {
                context.CompileContext.Trace.TraceError(Msg.C1030, id + Texts.__Boolean_parameter_must_have_value__True__or__False_, renderingTextNode, attributeName);
                value = "False";
            }

            var b = string.Equals(value, "True", StringComparison.OrdinalIgnoreCase);
            if (b == ignoreValue)
            {
                return;
            }

            output.WriteAttributeString(name, b ? "1" : "0");
        }

        protected virtual void WriteDataSource([NotNull] LayoutCompileContext context, [NotNull] XmlTextWriter output, [NotNull] ITextNode renderingTextNode)
        {
            var dataSource = renderingTextNode.GetAttributeValue("DataSource");
            if (string.IsNullOrEmpty(dataSource))
            {
                return;
            }

            var item = context.Field.Item.Project.FindQualifiedItem<IProjectItem>(dataSource);
            if (item == null)
            {
                context.CompileContext.Trace.TraceError(Msg.C1028, Texts.Datasource_not_found, dataSource);
                return;
            }

            output.WriteAttributeString("ds", item.Uri.Guid.Format());
        }

        protected virtual void WriteDevice([NotNull] LayoutCompileContext context, [NotNull] XmlTextWriter output, [NotNull, ItemNotNull] IEnumerable<Item> renderingItems, [NotNull] ITextNode deviceTextNode)
        {
            output.WriteStartElement("d");

            var deviceNameTextNode = deviceTextNode.GetAttribute("Name");
            if (deviceNameTextNode == null)
            {
                context.CompileContext.Trace.TraceError(Msg.C1029, Texts.Device_element_is_missing__Name__attribute_, deviceTextNode);
            }
            else
            {
                var devices = context.Field.Item.Project.ProjectItems.OfType<Item>().Where(i => string.Equals(i.TemplateIdOrPath, "/sitecore/templates/System/Layout/Device", StringComparison.OrdinalIgnoreCase) || string.Equals(i.TemplateIdOrPath, "{B6F7EEB4-E8D7-476F-8936-5ACE6A76F20B}", StringComparison.OrdinalIgnoreCase)).ToList();
                if (!devices.Any())
                {
                    context.CompileContext.Trace.TraceError(Msg.C1031, Texts.Device_item_not_found, deviceNameTextNode);
                }
                else
                {
                    var deviceName = deviceNameTextNode.Value;
                    var device = devices.FirstOrDefault(d => string.Equals(d.ItemName, deviceName, StringComparison.OrdinalIgnoreCase));
                    if (device == null)
                    {
                        context.CompileContext.Trace.TraceError(Msg.C1032, Texts.Device_not_found, deviceNameTextNode, deviceName);
                    }
                    else
                    {
                        output.WriteAttributeString("id", device.Uri.Guid.Format());
                    }
                }
            }

            var layoutPlaceholders = string.Empty;
            var layoutPath = deviceTextNode.GetAttribute("Layout");
            if (layoutPath != null && !string.IsNullOrEmpty(layoutPath.Value))
            {
                var l = context.Field.Item.Project.FindQualifiedItem<IProjectItem>(layoutPath.Value);
                if (l == null)
                {
                    context.CompileContext.Trace.TraceError(Msg.C1033, Texts.Layout_not_found_, layoutPath, layoutPath.Value);
                    return;
                }

                output.WriteAttributeString("l", l.Uri.Guid.Format());
                layoutPlaceholders = GetPlaceholders(context, deviceTextNode, l);
            }

            var renderings = deviceTextNode.GetSnapshotLanguageSpecificChildNode("Renderings");
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

        protected virtual void WriteLayout([NotNull] LayoutCompileContext context, [NotNull] XmlTextWriter output, [NotNull] ITextNode layoutTextNode)
        {
            var databaseName = context.Field.Item.DatabaseName;

            var renderingItems = context.Field.Item.Project.ProjectItems.OfType<Rendering>().Where(r => string.Equals(r.RenderingItemUri.FileOrDatabaseName, databaseName, StringComparison.OrdinalIgnoreCase)).Select(r => context.Field.Item.Project.FindQualifiedItem<Item>(r.RenderingItemUri)).ToList();
            renderingItems.AddRange(context.Field.Item.Project.ProjectItems.OfType<Item>().Where(r => r.IsImport && string.Equals(r.DatabaseName, databaseName, StringComparison.OrdinalIgnoreCase) && string.Equals(r.TemplateIdOrPath, "/sitecore/templates/System/Layout/Renderings/View rendering", StringComparison.OrdinalIgnoreCase)));

            output.WriteStartElement("r");

            var devices = layoutTextNode.GetSnapshotLanguageSpecificChildNode("Devices");
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

        protected virtual void WritePlaceholder([NotNull] LayoutCompileContext context, [NotNull] XmlTextWriter output, [NotNull] ITextNode renderingTextNode, [NotNull] string id, [NotNull] string parentPlaceholders)
        {
            var placeholderTextNode = renderingTextNode.GetAttribute("Placeholder");
            var placeholder = placeholderTextNode?.Value ?? string.Empty;

            // get the first placeholder from the parent rendering
            if (string.IsNullOrEmpty(placeholder) && !string.IsNullOrEmpty(parentPlaceholders))
            {
                var n = parentPlaceholders.IndexOf(",", 1, StringComparison.InvariantCultureIgnoreCase);
                if (n >= 0)
                {
                    placeholder = parentPlaceholders.Mid(1, n - 1);
                }
            }

            if (string.IsNullOrEmpty(placeholder))
            {
                return;
            }

            if (placeholderTextNode != null && !string.IsNullOrEmpty(parentPlaceholders))
            {
                if (parentPlaceholders.IndexOf("," + placeholder + ",", StringComparison.InvariantCultureIgnoreCase) < 0)
                {
                    var text = parentPlaceholders.Mid(1, parentPlaceholders.Length - 2);
                    if (string.IsNullOrEmpty(text))
                    {
                        text = "[None]";
                    }

                    context.CompileContext.Trace.TraceWarning(Msg.C1034, string.Format(Texts._2___Placeholder___0___is_not_defined_in_the_parent_rendering__Parent_rendering_has_these_placeholders___1__, placeholder, text, id), placeholderTextNode);
                }
            }

            output.WriteAttributeString("ph", placeholder);
        }

        protected virtual void WriteRendering([NotNull] LayoutCompileContext context, [NotNull] XmlTextWriter output, [NotNull, ItemNotNull] IEnumerable<Item> renderingItems, [NotNull] ITextNode renderingTextNode, [NotNull] string placeholders)
        {
            string renderingItemId;

            if (renderingTextNode.Key == "r")
            {
                renderingItemId = renderingTextNode.GetAttributeValue("id");
            }
            else if (renderingTextNode.Key == "Rendering")
            {
                renderingItemId = renderingTextNode.GetAttributeValue("RenderingName");
            }
            else
            {
                renderingItemId = renderingTextNode.Key.UnescapeXmlElementName();
            }

            var id = renderingTextNode.GetAttributeValue("Id");
            if (string.IsNullOrEmpty(id))
            {
                id = renderingItemId;
            }

            if (string.IsNullOrEmpty(renderingItemId))
            {
                context.CompileContext.Trace.TraceError(Msg.C1035, $"Unknown element \"{id}\".", renderingTextNode);
                return;
            }

            Item renderingItem;
            if (renderingItemId.IsGuid())
            {
                renderingItem = context.Field.Item.Project.ProjectItems.OfType<Item>().FirstOrDefault(i => i.Uri.Guid.Format() == renderingItemId);
            }
            else
            {
                var matches = ResolveRenderingItemId(renderingItems, renderingItemId);

                if (matches.Length == 0)
                {
                    context.CompileContext.Trace.TraceError(Msg.C1036, $"Rendering \"{renderingItemId}\" not found.", renderingTextNode);
                    return;
                }

                if (matches.Length > 1)
                {
                    context.CompileContext.Trace.TraceError(Msg.C1037, $"Ambiguous rendering match. {matches.Length} renderings match \"{renderingItemId}\".", renderingTextNode);
                    return;
                }

                renderingItem = matches[0];
            }

            if (renderingItem == null)
            {
                context.CompileContext.Trace.TraceError(Msg.C1038, $"Rendering \"{renderingItemId}\" not found.", renderingTextNode);
                return;
            }

            output.WriteStartElement("r");

            WriteBool(context, output, renderingTextNode, id, "Cacheable", "cac");
            output.WriteAttributeString("id", renderingItem.Uri.Guid.Format());
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

            var renderingsTextNode = renderingTextNode.GetSnapshotLanguageSpecificChildNode("Renderings");
            if (renderingsTextNode == null)
            {
                // silent
                return;
            }

            var renderingPlaceholders = GetPlaceholders(context, renderingTextNode, renderingItem);

            foreach (var childNode in renderingsTextNode.ChildNodes)
            {
                if (!IsContentProperty(renderingTextNode, childNode))
                {
                    WriteRendering(context, output, renderingItems, childNode, renderingPlaceholders);
                }
            }
        }

        [NotNull, ItemNotNull]
        private IEnumerable<string> AnalyzeFile([NotNull] LayoutCompileContext context, [NotNull] Item item, bool includeDynamicPlaceholders)
        {
            var pathField = item.Fields.FirstOrDefault(f => string.Equals(f.FieldName, "Path", StringComparison.OrdinalIgnoreCase));
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

        [ItemNotNull, NotNull]
        private IEnumerable<string> AnalyzeViewFile([NotNull] string source)
        {
            var matches = Regex.Matches(source, "\\@Html\\.Sitecore\\(\\)\\.Placeholder\\(\"([^\"]*)\"\\)", RegexOptions.IgnoreCase);
            return matches.OfType<Match>().Select(i => i.Groups[1].ToString().Trim());
        }

        [ItemNotNull, NotNull]
        private IEnumerable<string> AnalyzeViewFileWithDynamicPlaceholders([NotNull] string source)
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

        [ItemNotNull, NotNull]
        private IEnumerable<string> AnalyzeWebFormsFile([NotNull] string source)
        {
            var matches = Regex.Matches(source, "<[^>]*Placeholder[^>]*Key=\"([^\"]*)\"[^>]*>", RegexOptions.IgnoreCase);

            return matches.OfType<Match>().Select(i => i.Groups[1].ToString().Trim());
        }

        private void WriteParameters([NotNull] LayoutCompileContext context, [NotNull] XmlTextWriter output, [NotNull] ITextNode renderingTextNode, [NotNull] Item renderingItem, [NotNull] string id)
        {
            var fields = new Dictionary<string, string>();

            var parametersTemplateItemId = renderingItem.Fields.FirstOrDefault(f => f.FieldName == "Parameters Template")?.Value ?? string.Empty;
            var parametersTemplateItem = context.Field.Item.Project.FindQualifiedItem<Template>(parametersTemplateItemId);
            if (parametersTemplateItem != null)
            {
                foreach (var field in parametersTemplateItem.GetAllFields())
                {
                    fields[field.FieldName.ToLowerInvariant()] = field.Type;
                }
            }

            var properties = new Dictionary<string, string>();

            foreach (var attribute in renderingTextNode.Attributes)
            {
                properties[attribute.Key] = attribute.Value;
            }

            foreach (var child in renderingTextNode.ChildNodes)
            {
                if (IsContentProperty(renderingTextNode, child))
                {
                    var name = child.Key.Mid(renderingTextNode.Key.Length + 1);
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
                                    context.CompileContext.Trace.TraceError(Msg.C1039, $"{id}: Boolean parameter must have value \"True\", \"False\", \"{{Binding ... }}\" or \"{{@ ... }}\".", renderingTextNode, attributeName);
                                }

                                value = value == "1" || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ? "1" : "0";
                            }

                            break;
                    }
                }
                else
                {
                    context.CompileContext.Trace.TraceWarning(Msg.C1040, Texts._1___Parameter___0___is_not_defined_in_the_parameters_template_, renderingTextNode, id + "." + attributeName);
                }

                if (value.StartsWith("/sitecore", StringComparison.InvariantCultureIgnoreCase))
                {
                    var item = context.Field.Item.Project.FindQualifiedItem<IProjectItem>(value);
                    if (item != null)
                    {
                        value = item.Uri.Guid.Format();
                    }
                }

                par[attributeName] = value;
            }

            output.WriteAttributeString("par", par.ToString());
        }
    }
}
