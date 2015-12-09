// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Languages.Xml
{
    public static class FormatExtensions
    {
        public static void WriteAsContentXml([NotNull] this Item item, [NotNull] XmlTextWriter output, [CanBeNull] Action<XmlTextWriter> writeInner = null)
        {
            output.WriteStartElement(item.Template.ItemName.EscapeXmlElementName());
            output.WriteAttributeString("xmlns", "http://www.sitecore.net/pathfinder/content/" + item.DatabaseName.ToLowerInvariant());
            output.WriteAttributeString("Name", item.ItemName);
            output.WriteAttributeStringIf("Id", item.Uri.Guid.Format());
            output.WriteAttributeStringIf("Database", item.DatabaseName);
            output.WriteAttributeStringIf("Template", item.TemplateIdOrPath);

            // todo: write parent item path

            var sharedFields = item.Fields.Where(f => string.IsNullOrEmpty(f.Language) && f.Version == 0).ToList();
            var unversionedFields = item.Fields.Where(f => !string.IsNullOrEmpty(f.Language) && f.Version == 0).ToList();
            var versionedFields = item.Fields.Where(f => !string.IsNullOrEmpty(f.Language) && f.Version != 0).ToList();

            foreach (var field in sharedFields)
            {
                output.WriteAttributeString(field.FieldName, field.Value);
            }

            foreach (var language in unversionedFields.Select(f => f.Language).Distinct())
            {
                output.WriteStartElement("Fields.Unversioned");
                output.WriteAttributeString("Language", language);

                foreach (var field in unversionedFields.Where(f => f.Language == language))
                {
                    output.WriteAttributeString(field.FieldName, field.Value);
                }

                output.WriteEndElement();
            }

            foreach (var language in versionedFields.Select(f => f.Language).Distinct())
            {
                foreach (var version in versionedFields.Where(f => f.Language == language).Select(f => f.Version).Distinct())
                {
                    output.WriteStartElement("Fields.Versioned");
                    output.WriteAttributeString("Language", language);
                    output.WriteAttributeString("Version", version.ToString());

                    foreach (var field in versionedFields.Where(f => f.Language == language && f.Version == version))
                    {
                        output.WriteAttributeString(field.FieldName, field.Value);
                    }

                    output.WriteEndElement();
                }
            }

            if (writeInner != null)
            {
                writeInner(output);
            }

            output.WriteEndElement();
        }

        public static void WriteAsExportXml([NotNull] this Item item, [NotNull] XmlTextWriter output, [ItemNotNull] [NotNull] IEnumerable<string> fieldsToWrite)
        {
            output.WriteStartElement("Item");
            output.WriteAttributeString("Id", item.Uri.Guid.Format());
            output.WriteAttributeString("Database", item.DatabaseName);
            output.WriteAttributeString("Name", item.ItemName);
            output.WriteAttributeString("Path", item.ItemIdOrPath);
            output.WriteAttributeString("Template", item.TemplateIdOrPath);

            foreach (var field in item.Fields)
            {
                if (!fieldsToWrite.Contains(field.FieldName.ToLowerInvariant()))
                {
                    continue;
                }

                output.WriteStartElement("Field");
                output.WriteAttributeString("Name", field.FieldName);
                output.WriteAttributeString("Value", field.Value);
                output.WriteEndElement();
            }

            output.WriteEndElement();
        }

        public static void WriteAsExportXml([NotNull] this Template template, [NotNull] XmlTextWriter output)
        {
            output.WriteStartElement("Template");
            output.WriteAttributeString("Id", template.Uri.Guid.Format());
            output.WriteAttributeString("Database", template.DatabaseName);
            output.WriteAttributeString("Name", template.ItemName);
            output.WriteAttributeString("Path", template.ItemIdOrPath);
            output.WriteAttributeString("BaseTemplates", template.BaseTemplates);

            foreach (var section in template.Sections)
            {
                output.WriteStartElement("Section");
                output.WriteAttributeString("Name", section.SectionName);

                foreach (var field in section.Fields)
                {
                    output.WriteStartElement("Field");
                    output.WriteAttributeString("Name", field.FieldName);
                    output.WriteAttributeString("Type", field.Type);
                    output.WriteEndElement();
                }

                output.WriteEndElement();
            }

            output.WriteEndElement();
        }

        public static void WriteAsXml([NotNull] this LayoutBuilder layoutBuilder, [NotNull] TextWriter writer, [NotNull] string databaseName)
        {
            var output = new XmlTextWriter(writer)
            {
                Formatting = Formatting.Indented
            };

            output.WriteStartElement("Layout");
            output.WriteAttributeString("xmlns", "http://www.sitecore.net/pathfinder/layouts/" + databaseName);

            foreach (var deviceBuilder in layoutBuilder.Devices)
            {
                output.WriteStartElement("Device");
                output.WriteAttributeString("Name", deviceBuilder.DeviceName);
                output.WriteAttributeStringIf("Layout", deviceBuilder.LayoutItemPath);

                foreach (var renderingBuilder in deviceBuilder.Renderings.Where(r => r.ParentRendering == null))
                {
                    WriteAsXml(output, deviceBuilder, renderingBuilder);
                }

                output.WriteEndElement();
            }

            output.WriteEndElement();
        }

        public static void WriteAsXml([NotNull] this Item item, [NotNull] TextWriter writer, [CanBeNull] Action<TextWriter> writeInner = null)
        {
            var output = new XmlTextWriter(writer);
            output.Formatting = Formatting.Indented;

            output.WriteStartElement("Item");
            output.WriteAttributeString("xmlns", "http://www.sitecore.net/pathfinder/item");
            output.WriteAttributeStringIf("Id", item.Uri.Guid.Format());
            output.WriteAttributeStringIf("Database", item.DatabaseName);
            output.WriteAttributeStringIf("Name", item.ItemName);
            output.WriteAttributeStringIf("Path", item.ItemIdOrPath);
            output.WriteAttributeStringIf("Template", item.TemplateIdOrPath);

            output.WriteStartElement("Fields");

            var sharedFields = item.Fields.Where(f => string.IsNullOrEmpty(f.Language) && f.Version == 0).ToList();
            var unversionedFields = item.Fields.Where(f => !string.IsNullOrEmpty(f.Language) && f.Version == 0).ToList();
            var versionedFields = item.Fields.Where(f => !string.IsNullOrEmpty(f.Language) && f.Version != 0).ToList();

            foreach (var field in sharedFields)
            {
                if (string.Equals(field.TemplateField.Type, "Layout", StringComparison.OrdinalIgnoreCase) || string.Equals(field.FieldName, "__Renderings", StringComparison.OrdinalIgnoreCase) || string.Equals(field.FieldName, "__Final Renderings", StringComparison.OrdinalIgnoreCase))
                {
                    field.Value.ToXElement()?.WriteTo(output);
                    continue;
                }

                output.WriteStartElement("Field");
                output.WriteAttributeString("Name", field.FieldName);

                output.WriteRaw(EscapeFieldValue(field.Value));
                output.WriteEndElement();
            }

            foreach (var language in unversionedFields.Select(f => f.Language).Distinct())
            {
                output.WriteStartElement("Unversioned");
                output.WriteAttributeString("Language", language);

                foreach (var field in unversionedFields.Where(f => f.Language == language))
                {
                    if (string.Equals(field.TemplateField.Type, "Layout", StringComparison.OrdinalIgnoreCase) || string.Equals(field.FieldName, "__Renderings", StringComparison.OrdinalIgnoreCase) || string.Equals(field.FieldName, "__Final Renderings", StringComparison.OrdinalIgnoreCase))
                    {
                        field.Value.ToXElement()?.WriteTo(output);
                        continue;
                    }

                    output.WriteStartElement("Field");
                    output.WriteAttributeString("Name", field.FieldName);
                    output.WriteRaw(EscapeFieldValue(field.Value));
                    output.WriteEndElement();
                }

                output.WriteEndElement();
            }

            foreach (var language in versionedFields.Select(f => f.Language).Distinct())
            {
                output.WriteStartElement("Versioned");
                output.WriteAttributeString("Language", language);

                foreach (var version in versionedFields.Where(f => f.Language == language).Select(f => f.Version).Distinct())
                {
                    output.WriteStartElement("Version");
                    output.WriteAttributeString("Number", version.ToString());

                    foreach (var field in versionedFields.Where(f => f.Language == language && f.Version == version))
                    {
                        if (string.Equals(field.TemplateField.Type, "Layout", StringComparison.OrdinalIgnoreCase) || string.Equals(field.FieldName, "__Renderings", StringComparison.OrdinalIgnoreCase) || string.Equals(field.FieldName, "__Final Renderings", StringComparison.OrdinalIgnoreCase))
                        {
                            field.Value.ToXElement()?.WriteTo(output);
                            continue;
                        }

                        output.WriteStartElement("Field");
                        output.WriteAttributeString("Name", field.FieldName);
                        output.WriteRaw(EscapeFieldValue(field.Value));
                        output.WriteEndElement();
                    }

                    output.WriteEndElement();
                }

                output.WriteEndElement();
            }

            output.WriteEndElement();

            if (writeInner != null)
            {
                writeInner(writer);
            }

            output.WriteEndElement();
        }

        public static void WriteAsXml([NotNull] this Template template, [NotNull] TextWriter writer)
        {
            var output = new XmlTextWriter(writer)
            {
                Formatting = Formatting.Indented
            };

            output.WriteStartElement("Template");
            output.WriteAttributeString("xmlns", "http://www.sitecore.net/pathfinder/item");
            output.WriteAttributeString("Name", template.ItemName);
            output.WriteAttributeStringIf("Id", template.Uri.Guid.Format());
            output.WriteAttributeStringIf("Database", template.DatabaseName);
            output.WriteAttributeStringIf("Path", template.ItemIdOrPath);
            output.WriteAttributeStringIf("BaseTemplates", template.BaseTemplates);
            output.WriteAttributeStringIf("ShortHelp", template.ShortHelp);
            output.WriteAttributeStringIf("LongHelp", template.LongHelp);

            foreach (var section in template.Sections)
            {
                output.WriteStartElement("Section");
                output.WriteAttributeString("Name", section.SectionName);
                output.WriteAttributeStringIf("Id", section.Uri.Guid.Format());
                output.WriteAttributeStringIf("Icon", section.Icon);

                foreach (var field in section.Fields)
                {
                    output.WriteStartElement("Field");
                    output.WriteAttributeString("Name", field.FieldName);
                    output.WriteAttributeStringIf("Id", field.Uri.Guid.Format());
                    output.WriteAttributeStringIf("Sortorder", field.SortOrder);
                    output.WriteAttributeStringIf("Type", field.Type);
                    output.WriteAttributeStringIf("Source", field.Source);
                    output.WriteAttributeStringIf("Sharing", field.Shared ? "Shared" : field.Unversioned ? "Unversioned" : string.Empty);
                    output.WriteAttributeStringIf("ShortHelp", field.ShortHelp);
                    output.WriteAttributeStringIf("LongHelp", field.LongHelp);

                    output.WriteEndElement();
                }

                output.WriteEndElement();
            }

            output.WriteEndElement();
        }

        [NotNull]
        private static string EscapeFieldValue([NotNull] string value)
        {
            // todo: hmm... unsure how to escape it
            return value.Replace("&", "&amp;");
        }

        private static void WriteAsXml([NotNull] XmlTextWriter output, [NotNull] DeviceBuilder deviceBuilder, [NotNull] RenderingBuilder renderingBuilder)
        {
            if (!renderingBuilder.UnsafeName)
            {
                output.WriteStartElement(renderingBuilder.Name);
            }
            else
            {
                output.WriteStartElement("Rendering");
                output.WriteAttributeString("RenderingName", renderingBuilder.Name);
            }

            output.WriteAttributeStringIf("Placeholder", renderingBuilder.Placeholder);
            output.WriteAttributeStringIf("Cacheable", renderingBuilder.Cacheable);
            output.WriteAttributeStringIf("VaryByData", renderingBuilder.VaryByData);
            output.WriteAttributeStringIf("VaryByDevice", renderingBuilder.VaryByDevice);
            output.WriteAttributeStringIf("VaryByLogin", renderingBuilder.VaryByLogin);
            output.WriteAttributeStringIf("VaryByParameters", renderingBuilder.VaryByParameters);
            output.WriteAttributeStringIf("VaryByQueryString", renderingBuilder.VaryByQueryString);
            output.WriteAttributeStringIf("VaryByUser", renderingBuilder.VaryByUser);

            foreach (var attribute in renderingBuilder.Attributes)
            {
                output.WriteAttributeString(attribute.Key, attribute.Value);
            }

            output.WriteAttributeStringIf("DataSource", renderingBuilder.DataSource);

            foreach (var child in deviceBuilder.Renderings)
            {
                if (child.ParentRendering == renderingBuilder)
                {
                    WriteAsXml(output, deviceBuilder, child);
                }
            }

            output.WriteEndElement();
        }
    }
}
