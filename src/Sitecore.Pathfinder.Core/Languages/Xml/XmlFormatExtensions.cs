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
    public static class XmlFormatExtensions
    {
        public static void WriteAsContentXml([NotNull] this Item item, [NotNull] TextWriter writer, [CanBeNull] Action<XmlWriter> writeInner = null)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true
            };

            using (var output = XmlWriter.Create(writer, settings))
            {
                output.WriteStartElement(item.Template.ItemName.EscapeXmlElementName());
                // output.WriteAttributeString("xmlns", "http://www.sitecore.net/pathfinder/content/" + item.DatabaseName.ToLowerInvariant());
                output.WriteAttributeString("Id", item.Uri.Guid.Format());
                output.WriteAttributeString("Database", item.DatabaseName);
                output.WriteAttributeString("Name", item.ItemName);
                output.WriteAttributeString("ItemPath", item.ItemIdOrPath);

                if (item.Template.ItemName.EscapeXmlElementName() != item.Template.ItemName)
                {
                    output.WriteAttributeString("TemplateName", item.Template.ItemName);
                }

                var sharedFields = item.Fields.Where(f => f.Language == Language.Undefined && f.Version == Projects.Items.Version.Undefined).ToList();
                var unversionedFields = item.Fields.Where(f => f.Language != Language.Undefined && f.Version == Projects.Items.Version.Undefined).ToList();
                var versionedFields = item.Fields.Where(f => f.Language != Language.Undefined && f.Version != Projects.Items.Version.Undefined).ToList();

                foreach (var field in sharedFields)
                {
                    output.WriteStartElement(field.FieldName.EscapeXmlElementName());
                    if (field.FieldName.EscapeXmlElementName() != field.FieldName)
                    {
                        output.WriteAttributeString("Name", field.FieldName);
                    }

                    output.WriteValue(field.Value);
                    output.WriteEndElement();
                }

                if (unversionedFields.Any() || versionedFields.Any())
                {
                    output.WriteStartElement("Versions");

                    foreach (var language in unversionedFields.Select(f => f.Language).Distinct())
                    {
                        output.WriteStartElement(language.LanguageName.EscapeXmlElementName());
                        if (language.LanguageName.EscapeXmlElementName() != language.LanguageName)
                        {
                            output.WriteAttributeString("Name", language.LanguageName);
                        }

                        foreach (var field in unversionedFields.Where(f => f.Language == language))
                        {
                            output.WriteStartElement(field.FieldName.EscapeXmlElementName());
                            if (field.FieldName.EscapeXmlElementName() != field.FieldName)
                            {
                                output.WriteAttributeString("Name", field.FieldName);
                            }

                            output.WriteValue(field.Value);
                            output.WriteEndElement();
                        }

                        foreach (var version in versionedFields.Where(f => f.Language == language).Select(f => f.Version).Distinct())
                        {
                            output.WriteStartElement("Version");
                            output.WriteAttributeString("Number", version.ToString());

                            foreach (var field in versionedFields.Where(f => f.Language == language && f.Version == version))
                            {
                                output.WriteStartElement(field.FieldName.EscapeXmlElementName());
                                if (field.FieldName.EscapeXmlElementName() != field.FieldName)
                                {
                                    output.WriteAttributeString("Name", field.FieldName);
                                }

                                output.WriteValue(field.Value);
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
                    writeInner(output);
                }

                output.WriteEndElement();
            }
        }
        public static void WriteAsUpdatePackageXml([NotNull] this Item item, [NotNull] TextWriter writer)
        {
            void WriteField(XmlWriter output, Field field)
            {
                output.WriteStartElement("field");
                output.WriteFullElementString("fieldid", field.FieldId.Format());
                output.WriteFullElementString("fieldname", field.FieldName);
                output.WriteFullElementString("fieldkey", field.FieldName.ToLowerInvariant());
                output.WriteFullElementString("fieldvalue", field.CompiledValue);
                output.WriteEndElement();
            }

            var settings = new XmlWriterSettings
            {
                Indent = true
            };

            using (var output = XmlWriter.Create(writer, settings))
            {
                var parent = item.GetParent();
                var parentId = parent != null ? parent.Uri.Guid.Format() : string.Empty;

                output.WriteStartElement("addItemCommand");

                output.WriteFullElementString("collisionbehavior", string.Empty);
                output.WriteFullElementString("databasename", item.Database.DatabaseName);
                output.WriteFullElementString("itemid", item.Uri.Guid.Format());
                output.WriteFullElementString("itempath", item.ItemIdOrPath);
                output.WriteFullElementString("parent", parentId);

                output.WriteStartElement("item");

                output.WriteFullElementString("parent", parentId);
                output.WriteFullElementString("name", item.ItemName);
                output.WriteFullElementString("master", Constants.NullGuidString);
                output.WriteFullElementString("template", item.Template.Uri.Guid.Format());
                output.WriteFullElementString("templatekey", item.Template.ItemName);

                output.WriteStartElement("sharedfields");
                foreach (var sharedField in item.Fields.Where(f => f.TemplateField.Shared).ToArray())
                {
                    WriteField(output, sharedField);
                }

                output.WriteEndElement();

                output.WriteStartElement("versions");
                foreach (var language in item.Versions.GetLanguages())
                {
                    foreach (var version in item.Versions.GetVersions(language))
                    {
                        var isLatestVersion = item.Versions.IsLatestVersion(language, version);
                        var versionedFields = item.Fields.Where(f => !f.TemplateField.Shared && f.Language == language && (f.Version == version || isLatestVersion && f.Version == Projects.Items.Version.Latest));

                        var first = versionedFields.FirstOrDefault();
                        if (first == null)
                        {
                            continue;
                        }

                        output.WriteStartElement("itemversion");
                        output.WriteFullElementString("version", first.Version.Number.ToString());
                        output.WriteFullElementString("language", first.Language.LanguageName);
                        output.WriteFullElementString("revision", Guid.NewGuid().Format());

                        output.WriteStartElement("fields");

                        foreach (var versionedField in versionedFields)
                        {
                            WriteField(output, versionedField);
                        }

                        output.WriteEndElement();
                        output.WriteEndElement();
                    }
                }

                output.WriteEndElement();

                output.WriteEndElement(); // "item"
                output.WriteEndElement(); // "addItemCommand"
            }
        }

        public static void WriteAsXml([NotNull] this LayoutBuilder layoutBuilder, [NotNull] TextWriter writer, [NotNull] string databaseName)
        {
            var settings = new XmlWriterSettings()
            {
              Indent = true
            };
            var output = XmlWriter.Create(writer, settings);

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
            var settings = new XmlWriterSettings()
            {
                Indent = true
            };
            var output = XmlWriter.Create(writer, settings);

            output.WriteStartElement("Item");
            output.WriteAttributeString("xmlns", "http://www.sitecore.net/pathfinder/item");
            output.WriteAttributeStringIf("Id", item.Uri.Guid.Format());
            output.WriteAttributeStringIf("Database", item.DatabaseName);
            output.WriteAttributeStringIf("Name", item.ItemName);
            output.WriteAttributeStringIf("ItemPath", item.ItemIdOrPath);
            output.WriteAttributeStringIf("Template", item.TemplateIdOrPath);

            output.WriteStartElement("Fields");

            var sharedFields = item.Fields.Where(f => f.Language == Language.Undefined && f.Version == Projects.Items.Version.Undefined).ToList();
            var unversionedFields = item.Fields.Where(f => f.Language != Language.Undefined && f.Version == Projects.Items.Version.Undefined).ToList();
            var versionedFields = item.Fields.Where(f => f.Language != Language.Undefined && f.Version != Projects.Items.Version.Undefined).ToList();

            foreach (var field in sharedFields)
            {
                WriteField(output, field);
            }

            foreach (var language in unversionedFields.Select(f => f.Language).Distinct())
            {
                output.WriteStartElement("Unversioned");
                output.WriteAttributeString("Language", language.LanguageName);

                foreach (var field in unversionedFields.Where(f => f.Language == language))
                {
                    WriteField(output, field);
                }

                output.WriteEndElement();
            }

            foreach (var language in versionedFields.Select(f => f.Language).Distinct())
            {
                output.WriteStartElement("Versioned");
                output.WriteAttributeString("Language", language.LanguageName);

                foreach (var version in versionedFields.Where(f => f.Language == language).Select(f => f.Version).Distinct())
                {
                    output.WriteStartElement("Version");
                    output.WriteAttributeString("Number", version.ToString());

                    foreach (var field in versionedFields.Where(f => f.Language == language && f.Version == version))
                    {
                        WriteField(output, field);
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

        private static void WriteField([NotNull] XmlWriter output, [NotNull] Field field)
        {
            output.WriteStartElement("Field");
            output.WriteAttributeString("Name", field.FieldName);

            var written = false;
            var value = field.Value;

            // handle checkboxes
            if (string.Equals(field.TemplateField.Type, "Checkbox", StringComparison.OrdinalIgnoreCase))
            {
                if (value == "1")
                {
                    value = "True";
                }
                else if (value == "0" || string.IsNullOrEmpty(value))
                {
                    value = "False";
                }
            }

            // if value is valid xml with a single root node, output it raw
            if (value.TrimStart().StartsWith("<"))
            {
                var xml = value.ToXElement();
                if (xml != null)
                {
                    xml.WriteTo(output);
                    written = true;
                }
            }

            if (!written)
            {
                if (value.IndexOf('<') >= 0)
                {
                    output.WriteCData(value);
                }
                else
                {
                    output.WriteValue(value);
                }
            }

            output.WriteEndElement();
        }

        public static void WriteAsXml([NotNull] this Template template, [NotNull] TextWriter writer)
        {
            var settings = new XmlWriterSettings()
            {
                Indent = true
            };
            var output = XmlWriter.Create(writer, settings);

            output.WriteStartElement("Template");
            output.WriteAttributeString("xmlns", "http://www.sitecore.net/pathfinder/item");
            output.WriteAttributeString("Name", template.ItemName);
            output.WriteAttributeStringIf("Id", template.Uri.Guid.Format());
            output.WriteAttributeStringIf("Database", template.DatabaseName);
            output.WriteAttributeStringIf("ItemPath", template.ItemIdOrPath);
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

        private static void WriteAsXml([NotNull] XmlWriter output, [NotNull] DeviceBuilder deviceBuilder, [NotNull] RenderingBuilder renderingBuilder)
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
