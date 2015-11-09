// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Languages.Xml
{
    public static class ItemExtensions
    {
        public static void WriteAsContentXml([NotNull] this Item item, [NotNull] TextWriter output, [CanBeNull] [ItemNotNull] IEnumerable<string> fieldsToWrite = null, [CanBeNull] Action<XmlTextWriter, Item, IEnumerable<string>> writeInner = null)
        {
            using (var writer = new XmlTextWriter(output))
            {
                var templateName = item.Template.ItemName.EscapeXmlElementName();

                var parentItemPath = string.Empty;
                if (!item.ItemIdOrPath.IsGuid())
                {
                    parentItemPath = PathHelper.GetItemParentPath(item.ItemIdOrPath);
                }

                writer.WriteStartElement(templateName);
                writer.WriteAttributeString("Id", item.ItemIdOrPath);
                writer.WriteAttributeString("Name", item.ItemName);
                writer.WriteAttributeString("Database", item.DatabaseName);
                writer.WriteAttributeString("ParentItemPath", parentItemPath);

                var writeAll = fieldsToWrite == null || (fieldsToWrite.Count() == 1 && fieldsToWrite.ElementAt(0) == "*");
                foreach (var field in item.Fields)
                {
                    if (!writeAll && !fieldsToWrite.Contains(field.FieldName.ToLowerInvariant()))
                    {
                        continue;
                    }

                    var fieldName = field.FieldName.EscapeXmlElementName();
                    writer.WriteAttributeString(fieldName, field.Value);
                }

                if (writeInner != null)
                {
                    writeInner(writer, item, fieldsToWrite);
                }

                writer.WriteEndElement();
            }
        }
    }
}
