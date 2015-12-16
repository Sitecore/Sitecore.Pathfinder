// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Languages.Json
{
    public static class FormatExtensions
    {
        public static void WriteAsJson([NotNull] this LayoutBuilder layoutBuilder, [NotNull] TextWriter writer)
        {
            var output = new JsonTextWriter(writer)
            {
                Formatting = Formatting.Indented
            };

            output.WriteStartObject("Layout");
            output.WriteStartArray("Devices");

            foreach (var deviceBuilder in layoutBuilder.Devices)
            {
                output.WriteStartObject();
                output.WritePropertyStringIf("Name", deviceBuilder.DeviceName);
                output.WritePropertyStringIf("Layout", deviceBuilder.LayoutItemPath);

                output.WriteStartArray("Renderings");

                foreach (var renderingBuilder in deviceBuilder.Renderings.Where(r => r.ParentRendering == null))
                {
                    WriteAsJson(output, deviceBuilder, renderingBuilder);
                }

                output.WriteEndArray();
                output.WriteEndObject();
            }

            output.WriteEndArray();
            output.WriteEndObject();
        }

        public static void WriteAsJson([NotNull] this Item item, [NotNull] TextWriter writer, [CanBeNull] Action<TextWriter> writeInner = null)
        {
            var output = new JsonTextWriter(writer)
            {
                Formatting = Formatting.Indented
            };

            output.WriteStartObject();

            output.WriteStartObject("Item");
            output.WritePropertyString("Name", item.ItemName);
            output.WritePropertyStringIf("Id", item.Uri.Guid.Format());
            output.WritePropertyStringIf("Database", item.DatabaseName);
            output.WritePropertyStringIf("Path", item.ItemIdOrPath);
            output.WritePropertyStringIf("Template", item.TemplateIdOrPath);

            output.WriteStartObject("Fields");

            var sharedFields = item.Fields.Where(f => string.IsNullOrEmpty(f.Language) && f.Version == 0).ToList();
            var unversionedFields = item.Fields.Where(f => !string.IsNullOrEmpty(f.Language) && f.Version == 0).ToList();
            var versionedFields = item.Fields.Where(f => !string.IsNullOrEmpty(f.Language) && f.Version != 0).ToList();

            foreach (var field in sharedFields)
            {
                if (string.Equals(field.TemplateField.Type, "Layout", StringComparison.OrdinalIgnoreCase) || string.Equals(field.FieldName, "__Renderings", StringComparison.OrdinalIgnoreCase) || string.Equals(field.FieldName, "__Final Renderings", StringComparison.OrdinalIgnoreCase))
                {
                    var layout = "{" + field.Value + "}";
                    var jToken = layout.ToJToken();
                    jToken?.First.WriteTo(output);
                    continue;
                }

                output.WritePropertyString(field.FieldName, field.Value);
            }

            if (unversionedFields.Any())
            {
                output.WriteStartObject("Unversioned");

                foreach (var language in unversionedFields.Select(f => f.Language).Distinct())
                {
                    output.WriteStartObject(language);

                    foreach (var field in unversionedFields.Where(f => f.Language == language))
                    {
                        output.WritePropertyString(field.FieldName, field.Value);
                    }

                    output.WriteEndObject();
                }

                output.WriteEndObject();
            }

            if (versionedFields.Any())
            {
                output.WriteStartObject("Versioned");

                foreach (var language in versionedFields.Select(f => f.Language).Distinct())
                {
                    output.WriteStartObject(language);

                    foreach (var version in versionedFields.Where(f => f.Language == language).Select(f => f.Version).Distinct())
                    {
                        output.WriteStartObject(version.ToString());

                        foreach (var field in versionedFields.Where(f => f.Language == language && f.Version == version))
                        {
                            output.WritePropertyString(field.FieldName, field.Value);
                        }

                        output.WriteEndObject();
                    }

                    output.WriteEndObject();
                }

                output.WriteEndObject();
            }

            output.WriteEndObject();

            if (writeInner != null)
            {
                writeInner(writer);
            }

            output.WriteEndObject();

            output.WriteEndObject();
        }

        public static void WriteAsJson([NotNull] this Template template, [NotNull] TextWriter writer)
        {
            var output = new JsonTextWriter(writer)
            {
                Formatting = Formatting.Indented
            };

            output.WriteStartObject();

            output.WriteStartObject("Template");
            output.WritePropertyString("Name", template.ItemName);
            output.WritePropertyStringIf("Id", template.Uri.Guid.Format());
            output.WritePropertyStringIf("Database", template.DatabaseName);
            output.WritePropertyStringIf("Path", template.ItemIdOrPath);
            output.WritePropertyStringIf("BaseTemplates", template.BaseTemplates);

            output.WriteStartObject("Sections");

            foreach (var section in template.Sections)
            {
                output.WriteStartObject(section.SectionName);
                output.WritePropertyStringIf("Id", section.Uri.Guid.Format());
                output.WritePropertyStringIf("Name", section.SectionName);
                output.WritePropertyStringIf("Icon", section.Icon);

                output.WriteStartObject("Fields");

                foreach (var field in section.Fields)
                {
                    output.WriteStartObject(field.FieldName);
                    output.WritePropertyStringIf("Id", field.Uri.Guid.Format());
                    output.WritePropertyStringIf("Name", field.FieldName);
                    output.WritePropertyStringIf("Sortorder", field.SortOrder);
                    output.WritePropertyStringIf("Type", field.Type);
                    output.WritePropertyStringIf("Source", field.Source);
                    output.WritePropertyStringIf("Sharing", field.Shared ? "Shared" : field.Unversioned ? "Unversioned" : string.Empty);
                    output.WritePropertyStringIf("ShortHelp", field.ShortHelp);
                    output.WritePropertyStringIf("LongHelp", field.LongHelp);
                    output.WriteEndObject();
                }

                output.WriteEndObject();

                output.WriteEndObject();
            }

            output.WriteEndObject();

            output.WriteEndObject();

            output.WriteEndObject();
        }

        private static void WriteAsJson([NotNull] JsonTextWriter output, [NotNull] DeviceBuilder deviceBuilder, [NotNull] RenderingBuilder renderingBuilder)
        {
            output.WriteStartObject();
            output.WriteStartObject(renderingBuilder.Name);

            output.WritePropertyStringIf("Placeholder", renderingBuilder.Placeholder);
            output.WritePropertyStringIf("Cacheable", renderingBuilder.Cacheable);
            output.WritePropertyStringIf("VaryByData", renderingBuilder.VaryByData);
            output.WritePropertyStringIf("VaryByDevice", renderingBuilder.VaryByDevice);
            output.WritePropertyStringIf("VaryByLogin", renderingBuilder.VaryByLogin);
            output.WritePropertyStringIf("VaryByParameters", renderingBuilder.VaryByParameters);
            output.WritePropertyStringIf("VaryByQueryString", renderingBuilder.VaryByQueryString);
            output.WritePropertyStringIf("VaryByUser", renderingBuilder.VaryByUser);

            foreach (var attribute in renderingBuilder.Attributes)
            {
                output.WritePropertyString(attribute.Key, attribute.Value);
            }

            output.WritePropertyStringIf("DataSource", renderingBuilder.DataSource);

            var children = deviceBuilder.Renderings.Where(c => c.ParentRendering == renderingBuilder);
            if (children.Any())
            {
                output.WriteStartArray("Renderings");

                foreach (var child in children)
                {
                    WriteAsJson(output, deviceBuilder, child);
                }

                output.WriteEndArray();
            }

            output.WriteEndObject();
            output.WriteEndObject();
        }
    }
}
