// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Xml;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class WriteExports : BuildTaskBase
    {
        [ImportingConstructor]
        public WriteExports([NotNull] IConfiguration configuration, [NotNull] IFileSystemService fileSystem) : base("write-exports")
        {
            Configuration = configuration;
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.D1015, Texts.Writing_package_exports___);

            var fieldToWrite = context.Configuration.GetStringList(Constants.Configuration.WriteExports.FieldsToWrite).Select(f => f.ToLowerInvariant()).ToList();
            var fileName = PathHelper.Combine(Configuration.GetProjectDirectory(), Configuration.GetString(Constants.Configuration.Output.Directory) + "\\" + context.Configuration.GetString(Constants.Configuration.WriteExports.FileName));

            FileSystem.CreateDirectoryFromFileName(fileName);

            var project = context.LoadProject();

            using (var writer = FileSystem.OpenStreamWriter(fileName))
            {
                var settings = new XmlWriterSettings
                {
                    Encoding = new UTF8Encoding(false),
                    Indent = true
                };

                using (var output = XmlWriter.Create(writer, settings))
                {
                    output.WriteStartElement("Exports");

                    foreach (var template in project.Templates)
                    {
                        WriteAsExportXml(output, template);
                    }

                    foreach (var item in project.Items)
                    {
                        WriteAsExportXml(output, item, fieldToWrite);
                    }

                    output.WriteEndElement();
                }
            }
        }

        public static void WriteAsExportXml([NotNull] XmlWriter output, [NotNull] Template template)
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
                output.WriteAttributeString("Id", section.Uri.Guid.Format());
                output.WriteAttributeString("Name", section.SectionName);

                foreach (var field in section.Fields)
                {
                    output.WriteStartElement("Field");
                    output.WriteAttributeString("Id", field.Uri.Guid.Format());
                    output.WriteAttributeString("Name", field.FieldName);
                    output.WriteAttributeString("Type", field.Type);
                    output.WriteEndElement();
                }

                output.WriteEndElement();
            }

            output.WriteEndElement();
        }


        public static void WriteAsExportXml([NotNull] XmlWriter output, [NotNull] Item item, [NotNull, ItemNotNull]  IEnumerable<string> fieldsToWrite)
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
    }
}
