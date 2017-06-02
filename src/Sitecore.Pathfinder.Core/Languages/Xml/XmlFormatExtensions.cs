// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Xml;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Languages.Xml
{
    public static class XmlFormatExtensions
    {
        public static void WriteAsXml([NotNull] this Item item, [NotNull] TextWriter writer, bool writeChildren = false)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true
            };

            void WriteField(XmlWriter output, Field field)
            {
                output.WriteStartElement(field.FieldName.EscapeXmlElementName());
                if (field.FieldName.EscapeXmlElementName() != field.FieldName)
                {
                    output.WriteAttributeString("Name", field.FieldName);
                }

                output.WriteValue(field.CompiledValue);
                output.WriteEndElement();
            }

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

                if (item.Fields.Any())
                {
                    output.WriteStartElement("Fields");

                    foreach (var field in item.Versions.GetSharedFields().OrderBy(f => f.FieldName))
                    {
                        WriteField(output, field);
                    }

                    foreach (var language in item.Versions.GetLanguages())
                    {
                        var unversionedFields = item.Versions.GetUnversionedFields(language);
                        var versions = item.Versions.GetVersions(language);

                        if (!unversionedFields.Any() && !versions.Any())
                        {
                            continue;
                        }

                        output.WriteStartElement(language.LanguageName.EscapeXmlElementName());
                        if (language.LanguageName.EscapeXmlElementName() != language.LanguageName)
                        {
                            output.WriteAttributeString("Name", language.LanguageName);
                        }

                        foreach (var field in unversionedFields.OrderBy(f => f.FieldName))
                        {
                            WriteField(output, field);
                        }

                        foreach (var version in versions.OrderByDescending(v => v.Number))
                        {
                            var versionedFields = item.Versions.GetVersionedFields(language, version);
                            if (!versionedFields.Any())
                            {
                                continue;
                            }

                            output.WriteStartElement("Version");
                            output.WriteAttributeString("Number", version.ToString());

                            foreach (var field in versionedFields.OrderBy(f => f.FieldName))
                            {
                                WriteField(output, field);
                            }

                            output.WriteEndElement();
                        }

                        output.WriteEndElement();
                    }

                    output.WriteEndElement();
                }

                if (writeChildren)
                {
                    var children = item.Children.ToArray();
                    if (children.Any())
                    {
                        output.WriteStartElement("Children");
                        foreach (var child in children)
                        {
                            child.WriteAsXml(writer, true);
                        }

                        output.WriteEndElement();
                    }
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
    }
}
