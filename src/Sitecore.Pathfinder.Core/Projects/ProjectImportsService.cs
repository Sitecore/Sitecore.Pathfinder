// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Xml.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Packaging.ProjectPackages;

namespace Sitecore.Pathfinder.Projects
{
    [Export]
    public class ProjectImportsService
    {
        [ImportingConstructor]
        public ProjectImportsService([NotNull] IConfiguration configuration, [NotNull] ITraceService trace, [NotNull] IFactoryService factory, [NotNull] IFileSystemService fileSystem, [NotNull] IProjectPackageService projectPackages)
        {
            Configuration = configuration;
            Trace = trace;
            Factory = factory;
            FileSystem = fileSystem;
            ProjectPackages = projectPackages;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [NotNull]
        protected IProjectPackageService ProjectPackages { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        public virtual void Import([NotNull] IProject project)
        {
            // todo: consider making this a pipeline
            ImportReferences(project);
        }

        protected virtual void ImportElements([NotNull] IProject project, [NotNull] string fileName, [NotNull] XElement root)
        {
            foreach (var element in root.Elements())
            {
                ImportElement(project, fileName, element);
            }
        }

        protected virtual void ImportReferenceFile([NotNull] IProject project, [NotNull] string fileName)
        {
            Trace.TraceInformation(Msg.I1012, "Importing references", Path.GetFileName(fileName));

            try
            {
                var doc = FileSystem.ReadXml(fileName);

                var root = doc.Root;
                if (root == null)
                {
                    Trace.TraceError(Msg.I1001, Texts.Could_not_read_exports_xml_in_dependency_package, fileName);
                    return;
                }

                ImportElements(project, fileName, root);
            }
            catch
            {
                Trace.TraceError(Msg.I1002, Texts.Could_not_read_exports_xml_in_dependency_package, fileName);
            }
        }

        protected virtual void ImportReferences([NotNull] IProject project)
        {
            foreach (var pair in Configuration.GetSubKeys(Constants.Configuration.References))
            {
                var id = Configuration.GetString(Constants.Configuration.References + ":" + pair.Key + ":id");
                var version = Configuration.GetString(Constants.Configuration.References + ":" + pair.Key + ":version");
                var fileName = Configuration.GetString(Constants.Configuration.References + ":" + pair.Key);

                if (!string.IsNullOrEmpty(fileName))
                {
                    fileName = PathHelper.Combine(Configuration.GetProjectDirectory(), fileName);
                }
                else
                {
                    fileName = Path.Combine(Configuration.GetToolsDirectory() + "\\files\\references", id + "." + version + ".exports.xml");
                }

                ImportReferenceFile(project, fileName);
            }
        }

        private void ImportElement([NotNull] IProject project, [NotNull] string fileName, [NotNull] XElement element)
        {
            Guid guid;
            if (!Guid.TryParse(element.GetAttributeValue("Id"), out guid))
            {
                Trace.TraceError(Msg.I1003, Texts.Failed_to_parse_Id_, fileName);
                return;
            }

            var databaseName = element.GetAttributeValue("Database");
            var itemName = element.GetAttributeValue("Name");
            var itemIdOrPath = element.GetAttributeValue("Path");

            switch (element.Name.LocalName)
            {
                case "Item":
                    var item = Factory.Item(project, guid, databaseName, itemName, itemIdOrPath, element.GetAttributeValue("Template"));
                    item.IsImport = true;
                    item.IsEmittable = false;

                    foreach (var field in element.Elements())
                    {
                        item.Fields.Add(Factory.Field(item, field.GetAttributeValue("Name"), field.GetAttributeValue("Value")));
                    }

                    project.AddOrMerge(item);
                    break;

                case "Template":
                    var template = Factory.Template(project, guid, databaseName, itemName, itemIdOrPath);
                    template.IsImport = true;
                    template.IsEmittable = false;
                    template.BaseTemplates = element.GetAttributeValue("BaseTemplates");

                    if (string.IsNullOrEmpty(template.BaseTemplates))
                    {
                        template.BaseTemplates = Constants.Templates.StandardTemplateId;
                    }

                    foreach (var sectionElement in element.Elements())
                    {
                        Guid sectionGuid;
                        if (!Guid.TryParse(sectionElement.GetAttributeValue("Id"), out sectionGuid))
                        {
                            Trace.TraceError(Msg.I1004, Texts.Failed_to_parse_Id_, fileName);
                            return;
                        }

                        var templateSection = Factory.TemplateSection(template, sectionGuid);
                        templateSection.SectionName = sectionElement.GetAttributeValue("Name");

                        foreach (var fieldElement in sectionElement.Elements())
                        {
                            Guid fieldGuid;
                            if (!Guid.TryParse(fieldElement.GetAttributeValue("Id"), out fieldGuid))
                            {
                                Trace.TraceError(Msg.I1005, Texts.Failed_to_parse_Id_, fileName);
                                return;
                            }

                            var templateField = Factory.TemplateField(template, fieldGuid);
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
