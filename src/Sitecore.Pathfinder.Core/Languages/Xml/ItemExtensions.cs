// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Languages.Xml
{
    public static class ItemExtensions
    {
        public static void WriteAsExport([NotNull] this Item item, [NotNull] XmlTextWriter output, [ItemNotNull] [NotNull] IEnumerable<string> fieldsToWrite)
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

        public static void WriteAsExport([NotNull] this Template template, [NotNull] XmlTextWriter output)
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

        public static void WriteAsContentXml([NotNull] this Item item, [NotNull] XmlTextWriter output, [CanBeNull] Action<XmlTextWriter> writeInner = null)
        {
            output.WriteStartElement(item.Template.ItemName.EscapeXmlElementName());
            output.WriteAttributeString("xmlns", "http://www.sitecore.net/pathfinder/content/" + item.DatabaseName.ToLowerInvariant());
            output.WriteAttributeString("Id", item.Uri.Guid.Format());
            output.WriteAttributeString("Database", item.DatabaseName);
            output.WriteAttributeString("Name", item.ItemName);
            output.WriteAttributeString("Template", item.TemplateIdOrPath);
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

        public static void WriteAsXml([NotNull] this Item item, [NotNull] XmlTextWriter output, [CanBeNull] Action<XmlTextWriter> writeInner = null)
        {
            output.WriteStartElement("Item");
            output.WriteAttributeString("xmlns", "http://www.sitecore.net/pathfinder/item");
            output.WriteAttributeString("Id", item.Uri.Guid.Format());
            output.WriteAttributeString("Database", item.DatabaseName);
            output.WriteAttributeString("Name", item.ItemName);
            output.WriteAttributeString("Path", item.ItemIdOrPath);
            output.WriteAttributeString("Template", item.TemplateIdOrPath);

            output.WriteStartElement("Fields");

            var sharedFields = item.Fields.Where(f => string.IsNullOrEmpty(f.Language) && f.Version == 0).ToList();
            var unversionedFields = item.Fields.Where(f => !string.IsNullOrEmpty(f.Language) && f.Version == 0).ToList();
            var versionedFields = item.Fields.Where(f => !string.IsNullOrEmpty(f.Language) && f.Version != 0).ToList();

            foreach (var field in sharedFields)
            {
                output.WriteStartElement("Field");
                output.WriteAttributeString("Name", field.FieldName);
                output.WriteValue(field.Value);
                output.WriteEndElement();
            }

            foreach (var language in unversionedFields.Select(f => f.Language).Distinct())
            {
                output.WriteStartElement("Unversioned");
                output.WriteAttributeString("Language", language);

                foreach (var field in unversionedFields.Where(f => f.Language == language))
                {
                    output.WriteStartElement("Field");
                    output.WriteAttributeString("Name", field.FieldName);
                    output.WriteValue(field.Value);
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
                        output.WriteStartElement("Field");
                        output.WriteAttributeString("Name", field.FieldName);
                        output.WriteValue(field.Value);
                        output.WriteEndElement();
                    }

                    output.WriteEndElement();
                }

                output.WriteEndElement();
            }

            output.WriteEndElement();

            if (writeInner != null)
            {
                writeInner(output);
            }

            output.WriteEndElement();
        }
    }
}
