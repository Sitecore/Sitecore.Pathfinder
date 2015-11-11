// © 2015 Sitecore Corporation A/S. All rights reserved.

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
    }
}
