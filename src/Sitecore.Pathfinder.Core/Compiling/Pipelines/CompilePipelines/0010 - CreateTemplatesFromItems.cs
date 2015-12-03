// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    public class CreateTemplatesFromItems : PipelineProcessorBase<CompilePipeline>
    {
        public CreateTemplatesFromItems() : base(10)
        {
        }

        protected override void Process(CompilePipeline pipeline)
        {
            // todo: consider if imports should be omitted
            var templateItems = pipeline.Project.ProjectItems.OfType<Item>().Where(i => i.TemplateIdOrPath == Constants.Templates.Template || string.Equals(i.TemplateIdOrPath, Constants.Templates.TemplatePath, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var templateItem in templateItems)
            {
                if (pipeline.Project.ProjectItems.OfType<Template>().Any(t => t.Uri == templateItem.Uri))
                {
                    continue;
                }

                CreateTemplate(pipeline.Context, pipeline.Project, templateItem);
            }
        }

        private void CreateTemplate([NotNull] ICompileContext context, [NotNull] IProject project, [NotNull] Item templateItem)
        {
            var template = context.Factory.Template(project, templateItem.Uri.Guid, templateItem.SourceTextNodes.First(), templateItem.DatabaseName, templateItem.ItemName, templateItem.ItemIdOrPath);
            template.IsEmittable = false;
            template.IsImport = templateItem.IsImport;

            var baseTemplateField = templateItem.Fields.FirstOrDefault(f => f.FieldName == "__Base template");
            if (baseTemplateField != null)
            {
                template.BaseTemplatesProperty.SetValue(baseTemplateField.ValueProperty);
            }

            var iconField = templateItem.Fields.FirstOrDefault(f => f.FieldName == "__Icon");
            if (iconField != null)
            {
                template.IconProperty.SetValue(iconField.ValueProperty);
            }

            foreach (var sectionItem in templateItem.GetChildren())
            {
                var templateSection = context.Factory.TemplateSection(template, sectionItem.Uri.Guid, sectionItem.SourceTextNodes.First());
                template.Sections.Add(templateSection);
                templateSection.SectionNameProperty.SetValue(sectionItem.ItemNameProperty);

                var sectionIconField = sectionItem.Fields.FirstOrDefault(f => f.FieldName == "__Icon");
                if (sectionIconField != null)
                {
                    templateSection.IconProperty.SetValue(sectionIconField.ValueProperty);
                }

                foreach (var fieldItem in sectionItem.GetChildren())
                {
                    var templateField = context.Factory.TemplateField(template, fieldItem.Uri.Guid, fieldItem.SourceTextNodes.First());
                    templateSection.Fields.Add(templateField);
                    templateField.FieldNameProperty.SetValue(fieldItem.ItemNameProperty);

                    var typeField = fieldItem.Fields.FirstOrDefault(f => f.FieldName == "Type");
                    if (typeField != null)
                    {
                        templateField.TypeProperty.SetValue(typeField.ValueProperty);
                    }
                }
            }

            project.AddOrMerge(template);
        }
    }
}
