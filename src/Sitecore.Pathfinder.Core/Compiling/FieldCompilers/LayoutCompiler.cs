// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public LayoutCompiler([NotNull] IFileSystemService fileSystem)
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [NotNull]
        public virtual string Compile([NotNull] LayoutCompileContext context, [NotNull] ITextNode textNode)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                OmitXmlDeclaration = true
            };

            var writer = new StringBuilder();
            using (var output = XmlWriter.Create(writer, settings))
            {
                WriteLayout(context, output, textNode);
            }

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

        protected virtual bool IsContentProperty([NotNull] ITextNode renderingTextNode, [NotNull] ITextNode childTextNode)
        {
            return childTextNode.Key.StartsWith(renderingTextNode.Key + ".");
        }

        [NotNull, ItemNotNull]
        protected virtual Item[] ResolveRenderingItem([NotNull, ItemNotNull] IEnumerable<Item> renderingItems, [NotNull] string renderingItemId)
        {
            var path = "/" + renderingItemId.Replace(".", "/");

            return renderingItems.Where(r => r.ItemIdOrPath.EndsWith(path, StringComparison.OrdinalIgnoreCase)).ToArray();
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

        protected virtual void WriteBool([NotNull] LayoutCompileContext context, [NotNull] XmlWriter output, [NotNull] ITextNode renderingTextNode, [NotNull] string id, [NotNull] string attributeName, [NotNull] string name, bool ignoreValue = false)
        {
            var value = renderingTextNode.GetAttributeValue(attributeName);
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (value != "True" && value != "False")
            {
                context.Trace.TraceError(Msg.C1030, id + Texts.__Boolean_parameter_must_have_value__True__or__False_, renderingTextNode, attributeName);
                value = "False";
            }

            var b = string.Equals(value, "True", StringComparison.OrdinalIgnoreCase);
            if (b == ignoreValue)
            {
                return;
            }

            output.WriteAttributeString(name, b ? "1" : "0");
        }

        protected virtual void WriteDataSource([NotNull] LayoutCompileContext context, [NotNull] XmlWriter output, [NotNull] ITextNode renderingTextNode)
        {
            var dataSource = renderingTextNode.GetAttributeValue("DataSource");
            if (string.IsNullOrEmpty(dataSource))
            {
                return;
            }

            var item = context.Project.Indexes.FindQualifiedItem<IProjectItem>(dataSource);
            if (item == null)
            {
                context.Trace.TraceError(Msg.C1028, Texts.Datasource_not_found, dataSource);
                return;
            }

            output.WriteAttributeString("ds", item.Uri.Guid.Format());
        }

        protected virtual void WriteDevice([NotNull] LayoutCompileContext context, [NotNull] XmlWriter output, [NotNull, ItemNotNull] IEnumerable<Item> renderingItems, [NotNull] ITextNode deviceTextNode)
        {
            output.WriteStartElement("d");

            var deviceName = deviceTextNode.Key;
            var layout = deviceTextNode.Value;
            if (string.Equals(deviceName, "Device", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(deviceName))
            {
                deviceName = deviceTextNode.GetAttributeValue("Name");
                layout = deviceTextNode.GetAttributeValue("Layout");
            }

            if (string.IsNullOrEmpty(deviceName))
            {
                context.Trace.TraceError(Msg.C1029, Texts.Device_element_is_missing__Name__attribute_, deviceTextNode);
            }
            else
            {
                var devices = context.Project.ProjectItems.OfType<Item>().Where(i => string.Equals(i.TemplateIdOrPath, "/sitecore/templates/System/Layout/Device", StringComparison.OrdinalIgnoreCase) || string.Equals(i.TemplateIdOrPath, "{B6F7EEB4-E8D7-476F-8936-5ACE6A76F20B}", StringComparison.OrdinalIgnoreCase)).ToList();
                if (!devices.Any())
                {
                    context.Trace.TraceError(Msg.C1031, Texts.Device_item_not_found, deviceTextNode);
                }
                else
                {
                    var device = devices.FirstOrDefault(d => string.Equals(d.ItemName, deviceName, StringComparison.OrdinalIgnoreCase));
                    if (device == null)
                    {
                        context.Trace.TraceError(Msg.C1032, Texts.Device_not_found, deviceTextNode, deviceName);
                    }
                    else
                    {
                        output.WriteAttributeString("id", device.Uri.Guid.Format());
                    }
                }
            }

            if (!string.IsNullOrEmpty(layout))
            {
                var l = context.Project.Indexes.FindQualifiedItem<IProjectItem>(layout);

                if (l == null)
                {
                    var layouts = ResolveRenderingItem(renderingItems, layout);
                    if (layouts.Length > 1)
                    {
                        context.Trace.TraceError(Msg.C1130, "Ambiguous layout", layout);
                        return;
                    }

                    l = layouts.FirstOrDefault();
                }

                if (l == null)
                {
                    context.Trace.TraceError(Msg.C1033, Texts.Layout_not_found_, layout);
                    return;
                }

                output.WriteAttributeString("l", l.Uri.Guid.Format());
            }

            foreach (var placeholderTextNode in deviceTextNode.ChildNodes)
            {
                WritePlaceholder(context, output, renderingItems, placeholderTextNode);
            }

            output.WriteEndElement();
        }

        protected virtual void WritePlaceholder([NotNull] LayoutCompileContext context, [NotNull] XmlWriter output, [NotNull, ItemNotNull] IEnumerable<Item> renderingItems, [NotNull] ITextNode placeholderTextNode)
        {
            var placeholderName = placeholderTextNode.Key;
            if (string.Equals(placeholderName, "Placeholder", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(placeholderName))
            {
                placeholderName = placeholderTextNode.Value;
            }

            if (string.IsNullOrEmpty(placeholderName))
            {
                placeholderName = placeholderTextNode.GetAttributeValue("Placeholder");
            }

            if (string.IsNullOrEmpty(placeholderName))
            {
                context.Trace.TraceError(Msg.C1129, "Placeholder name missing", placeholderTextNode);
                return;
            }

            if (!placeholderTextNode.ChildNodes.Any())
            {
                context.Trace.TraceWarning(Msg.C1131, "Placeholder contains no renderings", placeholderTextNode, placeholderName);
                return;
            }

            foreach (var renderingTextNode in placeholderTextNode.ChildNodes)
            {
                WriteRendering(context, output, renderingItems, placeholderName, renderingTextNode);
            }
        }

        protected virtual void WriteLayout([NotNull] LayoutCompileContext context, [NotNull] XmlWriter output, [NotNull] ITextNode layoutTextNode)
        {
            var databaseName = context.Database.DatabaseName;

            var renderingItems = context.Project.ProjectItems.OfType<Rendering>().Where(r => string.Equals(r.RenderingItemUri.FileOrDatabaseName, databaseName, StringComparison.OrdinalIgnoreCase)).Select(r => context.Project.Indexes.FindQualifiedItem<Item>(r.RenderingItemUri)).ToList();
            renderingItems.AddRange(context.Project.ProjectItems.OfType<Item>().Where(r => r.IsImport && string.Equals(r.DatabaseName, databaseName, StringComparison.OrdinalIgnoreCase) && (string.Equals(r.TemplateIdOrPath, "/sitecore/templates/System/Layout/Renderings/View rendering", StringComparison.OrdinalIgnoreCase) || string.Equals(r.TemplateIdOrPath, Constants.Templates.ViewRenderingId, StringComparison.OrdinalIgnoreCase))));

            var devices = layoutTextNode.GetSnapshotLanguageSpecificChildNode("Devices");
            if (devices == null)
            {
                // silent
                return;
            }

            output.WriteStartElement("r");

            foreach (var deviceTextNode in devices.ChildNodes)
            {
                WriteDevice(context, output, renderingItems, deviceTextNode);
            }

            output.WriteEndElement();
        }

        protected virtual void WriteRendering([NotNull] LayoutCompileContext context, [NotNull] XmlWriter output, [NotNull, ItemNotNull] IEnumerable<Item> renderingItems, [NotNull] string placeholderName,  [NotNull] ITextNode renderingTextNode)
        {
            string renderingItemId;

            if (renderingTextNode.Key == "r")
            {
                renderingItemId = renderingTextNode.GetAttributeValue("id");
            }
            else if (string.Equals(renderingTextNode.Key, "Rendering", StringComparison.OrdinalIgnoreCase))
            {
                renderingItemId = renderingTextNode.Value;
                if (string.IsNullOrEmpty(renderingItemId))
                {
                    renderingItemId = renderingTextNode.GetAttributeValue("Name");
                }
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
                context.Trace.TraceError(Msg.C1035, $"Unknown element \"{id}\".", renderingTextNode);
                return;
            }

            Item renderingItem;
            if (Guid.TryParse(renderingItemId, out Guid guid))
            {
                renderingItem = context.Project.ProjectItems.OfType<Item>().FirstOrDefault(i => i.Uri.Guid == guid && i.Database == context.Database);
            }
            else
            {
                var matches = ResolveRenderingItemId(renderingItems, renderingItemId);

                if (matches.Length == 0)
                {
                    context.Trace.TraceError(Msg.C1036, $"Rendering \"{renderingItemId}\" not found.", renderingTextNode);
                    return;
                }

                if (matches.Length > 1)
                {
                    context.Trace.TraceError(Msg.C1037, $"Ambiguous rendering match. {matches.Length} renderings match \"{renderingItemId}\".", renderingTextNode);
                    return;
                }

                renderingItem = matches[0];
            }

            if (renderingItem == null)
            {
                context.Trace.TraceError(Msg.C1038, $"Rendering \"{renderingItemId}\" not found.", renderingTextNode);
                return;
            }

            output.WriteStartElement("r");

            WriteBool(context, output, renderingTextNode, id, "Cacheable", "cac");
            output.WriteAttributeString("id", renderingItem.Uri.Guid.Format());
            WriteDataSource(context, output, renderingTextNode);
            WriteParameters(context, output, renderingTextNode, renderingItem, id);
            output.WriteAttributeString("ph", placeholderName);

            // WriteAttributeStringNotEmpty(@"uid", this.UniqueId);
            WriteBool(context, output, renderingTextNode, id, "VaryByData", "vbd");
            WriteBool(context, output, renderingTextNode, id, "VaryByDevice", "vbdev");
            WriteBool(context, output, renderingTextNode, id, "VaryByLogin", "vbl");
            WriteBool(context, output, renderingTextNode, id, "VaryByParameters", "vbp");
            WriteBool(context, output, renderingTextNode, id, "VaryByQueryString", "vbqs");
            WriteBool(context, output, renderingTextNode, id, "VaryByUser", "vbu");

            output.WriteEndElement();

            foreach (var placeholderTextNode in renderingTextNode.ChildNodes)
            {
                WritePlaceholder(context, output, renderingItems, placeholderTextNode);
            }
        }

        private void WriteParameters([NotNull] LayoutCompileContext context, [NotNull] XmlWriter output, [NotNull] ITextNode renderingTextNode, [NotNull] Item renderingItem, [NotNull] string id)
        {
            var fields = new Dictionary<string, string>();

            var parametersTemplateItemId = renderingItem.Fields.FirstOrDefault(f => f.FieldName == "Parameters Template")?.Value ?? string.Empty;
            var parametersTemplateItem = context.Project.Indexes.FindQualifiedItem<Template>(parametersTemplateItemId);
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
                                    context.Trace.TraceError(Msg.C1039, $"{id}: Boolean parameter must have value \"True\", \"False\", \"{{Binding ... }}\" or \"{{@ ... }}\".", renderingTextNode, attributeName);
                                }

                                value = value == "1" || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ? "1" : "0";
                            }

                            break;
                    }
                }
                else
                {
                    // todo: reenable this check
                    // context.Trace.TraceWarning(Msg.C1040, Texts._1___Parameter___0___is_not_defined_in_the_parameters_template_, renderingTextNode, id + "." + attributeName);
                }

                if (value.StartsWith("/sitecore", StringComparison.OrdinalIgnoreCase))
                {
                    var item = context.Project.Indexes.FindQualifiedItem<IProjectItem>(value);
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
