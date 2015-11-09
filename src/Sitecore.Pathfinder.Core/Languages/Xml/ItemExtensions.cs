// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
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
        public static void WriteAsContentXml([NotNull] this Item item, [NotNull] XmlTextWriter output, [CanBeNull] [ItemNotNull] IEnumerable<string> fieldsToWrite = null, [CanBeNull] Action<XmlTextWriter, Item, IEnumerable<string>> writeInner = null)
        {
            var templateName = item.Template.ItemName.EscapeXmlElementName();

            var parentItemPath = string.Empty;
            if (!item.ItemIdOrPath.IsGuid())
            {
                parentItemPath = PathHelper.GetItemParentPath(item.ItemIdOrPath);
            }

            output.WriteStartElement(templateName);
            output.WriteAttributeString("Id", item.ItemIdOrPath);
            output.WriteAttributeString("Name", item.ItemName);
            output.WriteAttributeString("Database", item.DatabaseName);
            output.WriteAttributeString("ParentItemPath", parentItemPath);

            var writeAll = fieldsToWrite == null || (fieldsToWrite.Count() == 1 && fieldsToWrite.ElementAt(0) == "*");
            foreach (var field in item.Fields)
            {
                if (!writeAll && !fieldsToWrite.Contains(field.FieldName.ToLowerInvariant()))
                {
                    continue;
                }

                var fieldName = field.FieldName.EscapeXmlElementName();
                output.WriteAttributeString(fieldName, field.Value);
            }

            if (writeInner != null)
            {
                writeInner(output, item, fieldsToWrite);
            }

            output.WriteEndElement();
        }
    }
}
