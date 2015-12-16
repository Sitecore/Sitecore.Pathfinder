// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.SecurityModel;
using Sitecore.Zip;

namespace Sitecore.Pathfinder.WebApi.SynchronizeWebsites
{
    public class ContentXmlSynchronizer : ISynchronizer
    {
        public bool CanSynchronize(IConfiguration configuration, string fileName)
        {
            return fileName.EndsWith(".content.xml", StringComparison.OrdinalIgnoreCase);
        }

        public void Synchronize(IConfiguration configuration, ZipWriter zip, string fileName, string configKey)
        {
            var databaseName = configuration.GetString(configKey + "database");
            var itemPath = configuration.GetString(configKey + "path");
            var fieldsToWrite = configuration.GetString(configKey + "fields").Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim().ToLowerInvariant()).ToList();

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

                WriteItem(output, item, fieldsToWrite, true);

                zip.AddEntry(fileName, Encoding.UTF8.GetBytes(writer.ToString()));
            }
        }

        private void WriteItem([Diagnostics.NotNull] XmlTextWriter output, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull][ItemNotNull] IEnumerable<string> fieldsToWrite, bool writeParentItemPath)
        {
            if (item.TemplateID == TemplateIDs.Template)
            {
                WriteTemplate(output, item, writeParentItemPath);
                return;
            }

            var templateName = item.TemplateName.EscapeXmlElementName();

            output.WriteStartElement(templateName);
            output.WriteAttributeString("Id", item.ID.ToString());
            output.WriteAttributeString("Name", item.Name);

            if (writeParentItemPath)
            {
                output.WriteAttributeString("Database", item.Database.Name);
                output.WriteAttributeString("ParentItemPath", item.Parent == null ? "/" : item.Parent.Paths.Path);
                output.WriteAttributeString(Constants.Fields.IsImport, "True");
            }

            var writeAll = fieldsToWrite.Count() == 1 && fieldsToWrite.ElementAt(0) == "*";
            foreach (Field field in item.Fields)
            {
                if (!writeAll)
                {
                    if (!fieldsToWrite.Contains(field.Name.ToLowerInvariant()))
                    {
                        continue;
                    }
                }

                var fieldName = field.Name.EscapeXmlElementName();
                output.WriteAttributeString(fieldName, field.Value);
            }

            foreach (Item child in item.Children)
            {
                WriteItem(output, child, fieldsToWrite, false);
            }

            output.WriteEndElement();
        }

        private void WriteTemplate([Diagnostics.NotNull] XmlTextWriter output, [Diagnostics.NotNull] Item item, bool writeParentItemPath)
        {
            output.WriteStartElement("Template");
            output.WriteAttributeString("Id", item.ID.ToString());
            output.WriteAttributeString("Name", item.Name);

            var baseTemplates = item[FieldIDs.BaseTemplate];
            if (!string.IsNullOrEmpty(baseTemplates) && baseTemplates != "{1930BBEB-7805-471A-A3BE-4858AC7CF696}")
            {
                output.WriteAttributeString("BaseTemplates", baseTemplates);
            }

            if (writeParentItemPath)
            {
                output.WriteAttributeString("Database", item.Database.Name);
                output.WriteAttributeString("ParentItemPath", item.Parent == null ? "/" : item.Parent.Paths.Path);
            }

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
