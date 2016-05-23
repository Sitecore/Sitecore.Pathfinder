// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.NuGet;
using Sitecore.SecurityModel;
using Sitecore.Zip;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(nameof(WriteWebsiteExports), typeof(IWebsiteTask))]
    public class WriteWebsiteExports : WebsiteTaskBase
    {
        public WriteWebsiteExports() : base("server:write-website-exports")
        {
        }

        public override void Run(IWebsiteTaskContext context)
        {
            TempFolder.EnsureFolder();

            var tempDirectory = Path.Combine(FileUtil.MapPath(TempFolder.Folder), "Pathfinder.Exports");
            if (Directory.Exists(tempDirectory))
            {
                FileUtil.DeleteDirectory(tempDirectory, true);
            }

            Directory.CreateDirectory(tempDirectory);

            var exportFileName = Path.Combine(FileUtil.MapPath(tempDirectory), "Pathfinder.Exports.zip");
            using (var zip = new ZipWriter(exportFileName))
            {
                foreach (var index in context.Configuration.GetSubKeys("write-website-exports"))
                {
                    var entryName = context.Configuration.GetString("write-website-exports:" + index.Key + ":filename");
                    var fileKey = "write-website-exports:" + index.Key + ":";

                    var fileName = Path.Combine(tempDirectory, PathHelper.NormalizeFilePath(entryName).TrimStart('\\'));

                    context.FileSystem.CreateDirectoryFromFileName(fileName);

                    WriteFile(context.Configuration, tempDirectory, fileName, fileKey);

                    zip.AddEntry(entryName, fileName);
                }
            }

            context.ActionResult = new FilePathResult(exportFileName, "application/zip");
        }

        protected virtual void WriteFile([Diagnostics.NotNull] IConfiguration configuration, [Diagnostics.NotNull] string tempDirectory, [Diagnostics.NotNull] string fileName, [Diagnostics.NotNull] string fileKey)
        {
            var sourceFileName = Path.ChangeExtension(fileName, ".xml");
            using (var writer = new StreamWriter(sourceFileName))
            {
                var output = new XmlTextWriter(writer)
                {
                    Formatting = Formatting.Indented
                };

                output.WriteStartElement("Exports");

                foreach (var index in configuration.GetSubKeys(fileKey + "queries"))
                {
                    var key = fileKey + "queries:" + index.Key;

                    var queryText = configuration.GetString(key + ":query");

                    var databaseName = configuration.GetString(key + ":database");
                    var database = Factory.GetDatabase(databaseName);

                    var fieldToWrite = configuration.GetString(key + ":fields").Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim().ToLowerInvariant()).ToList();

                    using (new SecurityDisabler())
                    {
                        foreach (var item in database.Query(queryText))
                        {
                            if (item.TemplateID == TemplateIDs.Template)
                            {
                                WriteTemplateAsExport(output, item);
                            }
                            else
                            {
                                WriteItemAsExport(output, item, fieldToWrite);
                            }
                        }
                    }
                }

                output.WriteEndElement();
            }

            var packageBuilder = new NuGetPackageBuilder();
            packageBuilder.CreateNugetPackage(tempDirectory, fileName, sourceFileName);
        }

        protected virtual void WriteItemAsExport([Diagnostics.NotNull] XmlTextWriter output, [Diagnostics.NotNull] Item item, [ItemNotNull, NotNull] IEnumerable<string> fieldsToWrite)
        {
            output.WriteStartElement("Item");
            output.WriteAttributeString("Id", item.ID.ToString());
            output.WriteAttributeString("Database", item.Database.Name);
            output.WriteAttributeString("Name", item.Name);
            output.WriteAttributeString("Path", item.Paths.Path);
            output.WriteAttributeString("Template", item.Template.InnerItem.Paths.Path);

            foreach (Field field in item.Fields)
            {
                if (!fieldsToWrite.Contains(field.Name.ToLowerInvariant()))
                {
                    continue;
                }

                output.WriteStartElement("Field");
                output.WriteAttributeString("Id", field.ID.ToString());
                output.WriteAttributeString("Name", field.Name);
                output.WriteAttributeString("Value", field.Value);
                output.WriteEndElement();
            }

            output.WriteEndElement();
        }

        protected virtual void WriteTemplateAsExport([Diagnostics.NotNull] XmlTextWriter output, [Diagnostics.NotNull] Item templateItem)
        {
            output.WriteStartElement("Template");
            output.WriteAttributeString("Id", templateItem.ID.ToString());
            output.WriteAttributeString("Database", templateItem.Database.Name);
            output.WriteAttributeString("Name", templateItem.Name);
            output.WriteAttributeString("Path", templateItem.Paths.Path);
            output.WriteAttributeString("BaseTemplates", templateItem[FieldIDs.BaseTemplate]);

            var template = TemplateManager.GetTemplate(templateItem.ID, templateItem.Database);

            var templateFields = template.GetFields(false).ToList();

            foreach (var section in templateFields.Select(f => f.Section).Distinct().OrderBy(i => i.Sortorder).ThenBy(i => i.Key))
            {
                output.WriteStartElement("Section");
                output.WriteAttributeString("Id", section.ID.ToString());
                output.WriteAttributeString("Name", section.Name);

                foreach (var field in section.GetFields().ToList().OrderBy(i => i.Sortorder).ThenBy(i => i.Key))
                {
                    output.WriteStartElement("Field");
                    output.WriteAttributeString("Id", field.ID.ToString());
                    output.WriteAttributeString("Name", field.Name);
                    output.WriteAttributeString("Type", field.Type);
                    output.WriteEndElement();
                }

                output.WriteEndElement();
            }

            output.WriteEndElement();
        }
    }
}
