// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using System.IO;
using System.Xml.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Projects
{
    [Export, Shared]
    public class ProjectImportsService
    {
        [ImportingConstructor]
        public ProjectImportsService([NotNull] IConfiguration configuration, [NotNull] ITraceService trace, [NotNull] IFactoryService factory, [NotNull] IFileSystemService fileSystem)
        {
            Configuration = configuration;
            Trace = trace;
            Factory = factory;
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        public virtual void Import([NotNull] IProject project)
        {
            // todo: consider making this a pipeline

            ImportReferencesFromConfiguration(project);

            ImportReferencesFromPackagesDirectory(project);

            ImportReferencesFromNodeModulesDirectory(project);
        }

        protected virtual void ImportElement([NotNull] IProject project, [NotNull] string fileName, [NotNull] string defaultDatabaseName, [NotNull] XElement element)
        {
            Guid guid;
            if (!Guid.TryParse(element.GetAttributeValue("Id"), out guid))
            {
                Trace.TraceError(Msg.I1003, Texts.Failed_to_parse_Id_, fileName);
                return;
            }

            var databaseName = element.GetAttributeValue("Database", defaultDatabaseName) ?? defaultDatabaseName;
            var itemIdOrPath = element.GetAttributeValue("Path");
            var itemName = itemIdOrPath.Mid(itemIdOrPath.LastIndexOf('/') + 1);

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
                            templateField.Type = fieldElement.GetAttributeValue("Type", "Single-Line Text") ?? "Single-Line Text";

                            switch (fieldElement.GetAttributeValue("Sharing"))
                            {
                                case "Unversioned":
                                    templateField.Unversioned = true;
                                    break;
                                case "Versioned":
                                    break;
                                default:
                                    templateField.Shared = true;
                                    break;
                            }

                            templateSection.Fields.Add(templateField);
                        }

                        template.Sections.Add(templateSection);
                    }

                    project.AddOrMerge(template);
                    break;
            }
        }

        protected virtual void ImportElements([NotNull] IProject project, [NotNull] string fileName, [NotNull] XElement root)
        {
            var defaultDatabaseName = root.GetAttributeValue("Database", "master") ?? "master";

            foreach (var element in root.Elements())
            {
                ImportElement(project, fileName, defaultDatabaseName, element);
            }
        }

        protected virtual void ImportReferencesFromFile([NotNull] IProject project, [NotNull] string fileName)
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

        protected virtual void ImportReferencesFromConfiguration([NotNull] IProject project)
        {
            foreach (var pair in Configuration.GetSubKeys(Constants.Configuration.References))
            {
                var id = pair.Key;
                var version = Configuration.GetString(Constants.Configuration.References + ":" + id);
                if (version == "disabled")
                {
                    continue;
                }

                var fileName = PathHelper.NormalizeFilePath(id);
                if (fileName.IndexOf('\\') >= 0)
                {
                    fileName = PathHelper.Combine(project.ProjectDirectory, fileName);
                    if (!fileName.EndsWith(".exports.xml", StringComparison.OrdinalIgnoreCase))
                    {
                        fileName += "." + version + ".exports.xml";
                    }
                }
                else
                {
                    fileName = Path.Combine(Configuration.GetToolsDirectory() + "\\files\\references", id + "." + version + ".exports.xml");
                }

                if (!FileSystem.FileExists(fileName))
                {
                    Trace.TraceError(Msg.I1013, "Reference file not found", fileName);
                    continue;
                }

                ImportReferencesFromFile(project, fileName);
            }
        }

        protected virtual void ImportReferencesFromNodeModulesDirectory([NotNull] IProject project)
        {
            var nodeModulesDirectory = Path.Combine(project.ProjectDirectory, "node_modules");
            if (FileSystem.DirectoryExists(nodeModulesDirectory))
            {
                foreach (var fileName in Directory.GetFiles(nodeModulesDirectory, "project.exports.xml", SearchOption.AllDirectories))
                {
                    ImportReferencesFromFile(project, fileName);
                }
            }
        }

        protected virtual void ImportReferencesFromPackagesDirectory([NotNull] IProject project)
        {
            var packagesDirectory = Path.Combine(project.ProjectDirectory, "packages");
            if (FileSystem.DirectoryExists(packagesDirectory))
            {
                foreach (var fileName in Directory.GetFiles(packagesDirectory, "project.exports.xml", SearchOption.AllDirectories))
                {
                    ImportReferencesFromFile(project, fileName);
                }
            }
        }
    }
}
