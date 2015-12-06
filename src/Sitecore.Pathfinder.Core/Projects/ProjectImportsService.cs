// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    [Export]
    public class ProjectImportsService
    {
        [ImportingConstructor]
        public ProjectImportsService([NotNull] IConfiguration configuration, [NotNull] IFactoryService factory, [NotNull] IFileSystemService fileSystem)
        {
            Factory = factory;
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public virtual void Import([NotNull] IProject project, [NotNull] IParseContext context)
        {
            string packagesDirectory;

            if (context.Configuration.GetBool(Constants.Configuration.BuildingWithNoConfig))
            {
                packagesDirectory = Path.Combine(context.Configuration.GetString(Constants.Configuration.ToolsDirectory), "files\\project\\sitecore.project\\packages");
            }
            else
            {
                packagesDirectory = PathHelper.NormalizeFilePath(context.Configuration.Get(Constants.Configuration.CopyDependenciesSourceDirectory));
                packagesDirectory = Path.Combine(project.Options.ProjectDirectory, packagesDirectory);
            }

            if (!FileSystem.DirectoryExists(packagesDirectory))
            {
                return;
            }

            foreach (var fileName in FileSystem.GetFiles(packagesDirectory, "*.nupkg", SearchOption.AllDirectories))
            {
                // todo: consider caching this
                using (var zip = ZipFile.OpenRead(fileName))
                {
                    var entry = zip.GetEntry("content/sitecore.project/exports.xml");
                    if (entry == null)
                    {
                        continue;
                    }

                    var reader = new StreamReader(entry.Open());
                    try
                    {
                        var doc = XDocument.Load(reader);

                        var root = doc.Root;
                        if (root == null)
                        {
                            context.Trace.TraceError(Msg.I1001, Texts.Could_not_read_exports_xml_in_dependency_package, fileName);
                            continue;
                        }

                        ImportElements(project, context, fileName, root);
                    }
                    catch
                    {
                        context.Trace.TraceError(Msg.I1002, Texts.Could_not_read_exports_xml_in_dependency_package, fileName);
                    }
                }
            }
        }

        protected virtual void ImportElements([NotNull] IProject project, [NotNull] IParseContext context, [NotNull] string fileName, [NotNull] XElement root)
        {
            foreach (var element in root.Elements())
            {
                ImportElement(project, context, fileName, element);
            }
        }

        private void ImportElement([NotNull] IProject project, [NotNull] IParseContext context, [NotNull] string fileName, [NotNull] XElement element)
        {
            Guid guid;
            if (!Guid.TryParse(element.GetAttributeValue("Id"), out guid))
            {
                context.Trace.TraceError(Msg.I1003, Texts.Failed_to_parse_Id_, fileName);
                return;
            }

            var databaseName = element.GetAttributeValue("Database");
            var itemName = element.GetAttributeValue("Name");
            var itemIdOrPath = element.GetAttributeValue("Path");

            switch (element.Name.LocalName)
            {
                case "Item":
                    var item = Factory.Item(project, TextNode.Empty, guid, databaseName, itemName, itemIdOrPath, element.GetAttributeValue("Template"));
                    item.IsImport = true;
                    item.IsEmittable = false;

                    foreach (var field in element.Elements())
                    {
                        item.Fields.Add(Factory.Field(item, TextNode.Empty, field.GetAttributeValue("Name"), field.GetAttributeValue("Value")));
                    }

                    project.AddOrMerge(item);
                    break;

                case "Template":
                    var template = Factory.Template(project, guid, TextNode.Empty, databaseName, itemName, itemIdOrPath);
                    template.IsImport = true;
                    template.IsEmittable = false;
                    template.BaseTemplates = element.GetAttributeValue("BaseTemplates");

                    foreach (var sectionElement in element.Elements())
                    {
                        Guid sectionGuid;
                        if (!Guid.TryParse(sectionElement.GetAttributeValue("Id"), out sectionGuid))
                        {
                            context.Trace.TraceError(Msg.I1004, Texts.Failed_to_parse_Id_, fileName);
                            return;
                        }

                        var templateSection = Factory.TemplateSection(template, sectionGuid, TextNode.Empty);
                        templateSection.SectionName = sectionElement.GetAttributeValue("Name");

                        foreach (var fieldElement in sectionElement.Elements())
                        {
                            Guid fieldGuid;
                            if (!Guid.TryParse(fieldElement.GetAttributeValue("Id"), out fieldGuid))
                            {
                                context.Trace.TraceError(Msg.I1005, Texts.Failed_to_parse_Id_, fileName);
                                return;
                            }

                            var templateField = Factory.TemplateField(template, fieldGuid, TextNode.Empty);
                            templateField.FieldName = fieldElement.GetAttributeValue("Name");
                            templateField.Type = fieldElement.GetAttributeValue("Type");

                            templateSection.Fields.Add(templateField);
                        }

                        template.Sections.Add(templateSection);
                    }

                    project.AddOrMerge(template);
                    break;
            }
        }
    }
}
