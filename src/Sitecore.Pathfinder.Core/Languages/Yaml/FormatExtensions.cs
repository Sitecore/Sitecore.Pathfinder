// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    public static class FormatExtensions
    {
        public static void WriteAsYaml([NotNull] this LayoutBuilder layoutBuilder, [NotNull] TextWriter writer)
        {
            var output = new YamlTextWriter(writer);

            output.WriteStartElement("Layout");

            foreach (var deviceBuilder in layoutBuilder.Devices)
            {
                output.WriteStartElement("Device");
                output.WriteAttributeString("Name", deviceBuilder.DeviceName);
                output.WriteAttributeStringIf("Layout", deviceBuilder.LayoutItemPath);

                foreach (var renderingBuilder in deviceBuilder.Renderings.Where(r => r.ParentRendering == null))
                {
                    WriteAsYaml(output, deviceBuilder, renderingBuilder);
                }

                output.WriteEndElement();
            }

            output.WriteEndElement();
        }

        public static void WriteAsYaml([NotNull] this Item item, [NotNull] TextWriter writer, [CanBeNull] Action<TextWriter, int> writeInner = null)
        {
            var output = new YamlTextWriter(writer);

            var sharedFields = item.Fields.Where(f => f.Language == Language.Undefined && f.Version == Projects.Items.Version.Undefined).ToList();
            var unversionedFields = item.Fields.Where(f => f.Language != Language.Undefined && f.Version == Projects.Items.Version.Undefined).ToList();
            var versionedFields = item.Fields.Where(f => f.Language != Language.Undefined && f.Version != Projects.Items.Version.Undefined).ToList();

            output.WriteStartElement("Item");

            output.WriteAttributeString("Id", item.Uri.Guid.Format());
            output.WriteAttributeStringIf("Database", item.DatabaseName);
            output.WriteAttributeStringIf("Name", item.ItemName);
            output.WriteAttributeStringIf("ItemPath", item.ItemIdOrPath);
            output.WriteAttributeStringIf("Template", item.TemplateIdOrPath);

            output.WriteStartElement("Fields");

            foreach (var field in sharedFields)
            {
                output.WriteStartElement("Field", field.FieldName);
                output.WriteAttributeString("Value", field.Value);
                output.WriteEndElement();
            }

            if (unversionedFields.Any())
            {
                output.WriteStartElement("Unversioned");

                foreach (var language in unversionedFields.Select(f => f.Language).Distinct())
                {
                    output.WriteStartElement(language.LanguageName);

                    foreach (var field in unversionedFields.Where(f => f.Language == language))
                    {
                        output.WriteStartElement("Field", field.FieldName);
                        output.WriteAttributeString("Value", field.Value);
                        output.WriteEndElement();
                    }

                    output.WriteEndElement();
                }

                output.WriteEndElement();
            }

            if (versionedFields.Any())
            {
                output.WriteStartElement("Versioned");

                foreach (var language in versionedFields.Select(f => f.Language).Distinct())
                {
                    output.WriteStartElement(language.LanguageName);

                    foreach (var version in versionedFields.Where(f => f.Language == language).Select(f => f.Version).Distinct())
                    {
                        output.WriteStartElement(version.ToString());

                        foreach (var field in versionedFields.Where(f => f.Language == language && f.Version == version))
                        {
                            output.WriteStartElement("Field", field.FieldName);
                            output.WriteAttributeString("Value", field.Value);
                            output.WriteEndElement();
                        }

                        output.WriteEndElement();
                    }

                    output.WriteEndElement();
                }

                output.WriteEndElement();
            }

            if (writeInner != null)
            {
                writeInner(writer, output.Indent);
            }
        }

        public static void WriteAsYaml([NotNull] this Template template, [NotNull] TextWriter writer)
        {
            var output = new YamlTextWriter(writer);

            output.WriteStartElement("Template");
            output.WriteAttributeString("Name", template.ItemName);
            output.WriteAttributeStringIf("Id", template.Uri.Guid.Format());
            output.WriteAttributeStringIf("Database", template.DatabaseName);
            output.WriteAttributeStringIf("ItemPath", template.ItemIdOrPath);
            output.WriteAttributeStringIf("BaseTemplates", template.BaseTemplates);

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
                    output.WriteAttributeStringIf("Sortorder", field.Sortorder);
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

        private static void WriteAsYaml([NotNull] YamlTextWriter output, [NotNull] DeviceBuilder deviceBuilder, [NotNull] RenderingBuilder renderingBuilder)
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
                    WriteAsYaml(output, deviceBuilder, child);
                }
            }

            output.WriteEndElement();
        }
    }
}
