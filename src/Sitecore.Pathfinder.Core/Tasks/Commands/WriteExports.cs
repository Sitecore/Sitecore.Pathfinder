// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Xml;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks.Commands
{
    [Export(typeof(ITask)), Shared]
    public class WriteExports : BuildTaskBase
    {
        [ImportingConstructor]
        public WriteExports([NotNull] IConfiguration configuration, [NotNull] IFactory factory, [NotNull] IFileSystem fileSystem) : base("write-exports")
        {
            Configuration = configuration;
            Factory = factory;
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactory Factory { get; }

        [NotNull]
        protected IFileSystem FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            var project = context.LoadProject();

            context.Trace.TraceInformation(Msg.D1015, Texts.Writing_package_exports___);

            var fieldToWrite = context.Configuration.GetArray(Constants.Configuration.WriteExports.FieldsToWrite).Select(f => f.ToLowerInvariant()).ToList();
            var fileName = PathHelper.Combine(context.ProjectDirectory, Configuration.GetString(Constants.Configuration.Output.Directory) + "\\" + context.Configuration.GetString(Constants.Configuration.WriteExports.FileName));

            FileSystem.CreateDirectoryFromFileName(fileName);

            using (var writer = FileSystem.OpenStreamWriter(fileName))
            {
                var defaultDatabase = context.Configuration.GetString(Constants.Configuration.Database, "master");

                using (var output = Factory.XmlWriter(writer))
                {
                    output.WriteStartElement("Exports");
                    output.WriteAttributeString("Database", defaultDatabase);

                    foreach (var template in project.Templates.Where(t => t.IsEmittable).OrderBy(t => t.ItemIdOrPath))
                    {
                        WriteAsExportXml(output, defaultDatabase, template);
                    }

                    foreach (var item in project.Items.Where(i => i.IsEmittable).OrderBy(i => i.ItemIdOrPath))
                    {
                        WriteAsExportXml(output, defaultDatabase, item, fieldToWrite);
                    }

                    output.WriteEndElement();
                }
            }
        }

        public static void WriteAsExportXml([NotNull] XmlWriter output, [NotNull] string defaultDatabase, [NotNull] Template template)
        {
            output.WriteStartElement("Template");
            output.WriteAttributeString("Id", template.Uri.Guid.Format());
            if (!string.Equals(template.DatabaseName, defaultDatabase, StringComparison.OrdinalIgnoreCase))
            {
                output.WriteAttributeString("Database", template.DatabaseName);
            }

            output.WriteAttributeString("Path", template.ItemIdOrPath);
            if (!string.IsNullOrEmpty(template.BaseTemplates))
            {
                output.WriteAttributeString("BaseTemplates", template.BaseTemplates);
            }

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
                    if (!string.Equals(field.Type, "text", StringComparison.OrdinalIgnoreCase) && !string.Equals(field.Type, "Single-Line Text", StringComparison.OrdinalIgnoreCase))
                    {
                        output.WriteAttributeString("Type", field.Type);
                    }

                    if (!field.Shared)
                    {
                        output.WriteAttributeString("Sharing", field.Unversioned ? "Unversioned" : "Versioned");
                    }

                    output.WriteEndElement();
                }

                output.WriteEndElement();
            }

            output.WriteEndElement();
        }


        public static void WriteAsExportXml([NotNull] XmlWriter output, [NotNull] string defaultDatabase, [NotNull] Item item, [NotNull, ItemNotNull] IEnumerable<string> fieldsToWrite)
        {
            var templateIdOrPath = item.TemplateIdOrPath;

            var template = item.Database.FindByIdOrPath<Template>(item.TemplateIdOrPath);
            if (template != null)
            {
                templateIdOrPath = template.Uri.Guid.Format();
            }

            output.WriteStartElement("Item");
            output.WriteAttributeString("Id", item.Uri.Guid.Format());
            if (!string.Equals(item.DatabaseName, defaultDatabase, StringComparison.OrdinalIgnoreCase))
            {
                output.WriteAttributeString("Database", item.DatabaseName);
            }

            output.WriteAttributeString("Template", templateIdOrPath);
            output.WriteAttributeString("Path", item.ItemIdOrPath);

            foreach (var fieldName in fieldsToWrite)
            {
                var field = item.Fields[fieldName];
                if (field == null)
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
