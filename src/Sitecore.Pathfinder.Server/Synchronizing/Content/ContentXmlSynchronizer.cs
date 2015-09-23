// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.SecurityModel;
using Sitecore.Zip;

namespace Sitecore.Pathfinder.Synchronizing.Content
{
    [Export(typeof(ISynchronizer))]
    public class ContentXmlSynchronizer : ISynchronizer
    {
        // todo: make configurable
        [Diagnostics.NotNull]
        [ItemNotNull]
        private static readonly List<string> FieldsToWrite = new List<string>
        {
            "place holders"
        };

        public bool CanSynchronize(Microsoft.Framework.ConfigurationModel.Configuration configuration, string fileName)
        {
            return fileName.EndsWith(".content.xml", StringComparison.OrdinalIgnoreCase);
        }

        public void Synchronize(Microsoft.Framework.ConfigurationModel.Configuration configuration, ZipWriter zip, string fileName, string configKey)
        {
            var databaseName = configuration.GetString(configKey + "database");
            var itemPath = configuration.GetString(configKey + "path");

            var database = Factory.GetDatabase(databaseName);

            using (new SecurityDisabler())
            {
                var item = database.GetItem(itemPath);
                if (item == null)
                {
                    return;
                }

                var writer = new StringWriter();
                var output = new XmlTextWriter(writer)
                {
                    Formatting = Formatting.Indented
                };

                WriteItem(output, item, true);

                zip.AddEntry(fileName, Encoding.UTF8.GetBytes(writer.ToString()));
            }
        }

        private void WriteItem([Diagnostics.NotNull] XmlTextWriter output, [Diagnostics.NotNull] Item item, bool writeParentItemPath)
        {
            if (item.TemplateID == TemplateIDs.Template)
            {
                WriteTemplate(output, item, writeParentItemPath);
                return;
            }

            var templateName = item.TemplateName.Replace(" ", "--");

            output.WriteStartElement(templateName);
            output.WriteAttributeString("Id", item.ID.ToString());
            output.WriteAttributeString("Name", item.Name);

            if (writeParentItemPath)
            {
                output.WriteAttributeString("Database", item.Database.Name);
                output.WriteAttributeString("ParentItemPath", item.Parent == null ? "/" : item.Parent.Paths.Path);
            }

            output.WriteAttributeString("IsExternalReference", "True");

            foreach (Field field in item.Fields)
            {
                if (!FieldsToWrite.Contains(field.Name.ToLowerInvariant()))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(field.Value))
                {
                    continue;
                }

                var fieldName = field.Name.Replace(" ", "--");
                output.WriteAttributeString(fieldName, field.Value);
            }

            foreach (Item child in item.Children)
            {
                WriteItem(output, child, false);
            }

            output.WriteEndElement();
        }

        private void WriteTemplate([Diagnostics.NotNull] XmlTextWriter output, [Diagnostics.NotNull] Item item, bool writeParentItemPath)
        {
            output.WriteStartElement("Template");
            output.WriteAttributeString("Id", item.ID.ToString());
            output.WriteAttributeString("Name", item.Name);

            if (writeParentItemPath)
            {
                output.WriteAttributeString("Database", item.Database.Name);
                output.WriteAttributeString("ParentItemPath", item.Parent == null ? "/" : item.Parent.Paths.Path);
            }

            output.WriteAttributeString("IsExternalReference", "True");

            var templateItem = new TemplateItem(item);
            foreach (var templateSectionItem in templateItem.GetSections())
            {
                output.WriteStartElement("Section");
                output.WriteAttributeString("Id", templateSectionItem.ID.ToString());
                output.WriteAttributeString("Name", templateSectionItem.Name);

                foreach (var templateFieldItem in templateSectionItem.GetFields())
                {
                    output.WriteStartElement("Field");
                    output.WriteAttributeString("Id", templateFieldItem.ID.ToString());
                    output.WriteAttributeString("Name", templateFieldItem.Name);
                    output.WriteEndElement();
                }

                output.WriteEndElement();
            }

            output.WriteEndElement();
        }
    }
}
