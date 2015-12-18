// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using System.Reflection;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Languages.BinFiles.Pipelines
{
    public class ModelScanner : PipelineProcessorBase<BinFileCompilerPipeline>
    {
        public ModelScanner() : base(1000)
        {
        }

        protected override void Process(BinFileCompilerPipeline pipeline)
        {
            var typeName = pipeline.Type.FullName;
            if (!typeName.EndsWith("Model"))
            {
                return;
            }

            var snapshotTextNode = new SnapshotTextNode(pipeline.BinFile.Snapshots.First());

            var databaseName = pipeline.BinFile.Project.Options.DatabaseName;
            var itemName = pipeline.Type.Name;
            var itemIdOrPath = "/" + typeName.Replace('.', '/');
            var guid = StringHelper.GetGuid(pipeline.BinFile.Project, typeName);

            var template = pipeline.Context.Factory.Template(pipeline.BinFile.Project, guid, snapshotTextNode, databaseName, itemName, itemIdOrPath);
            template.IsEmittable = true;
            template.IsImport = false;

            guid = StringHelper.GetGuid(pipeline.BinFile.Project, itemIdOrPath + "/Fields");
            var templateSection = pipeline.Context.Factory.TemplateSection(template, guid, snapshotTextNode);
            templateSection.SectionName = "Fields";

            foreach (var propertyInfo in pipeline.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                guid = StringHelper.GetGuid(pipeline.BinFile.Project, itemIdOrPath + "/Fields/" + propertyInfo.Name);
                var templateField = pipeline.Context.Factory.TemplateField(template, guid, snapshotTextNode);
                templateField.FieldName = propertyInfo.Name;
                templateField.Type = "Single-line Text";

                if (propertyInfo.PropertyType == typeof(bool))
                {
                    templateField.Type = "Checkbox";
                }

                templateField.FieldName = propertyInfo.Name;

                templateSection.Fields.Add(templateField);
            }

            template.Sections.Add(templateSection);

            pipeline.BinFile.Project.AddOrMerge(template);
        }
    }
}
