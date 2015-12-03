// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    public class CreateItemsFromTemplates : PipelineProcessorBase<CompilePipeline>
    {
        public CreateItemsFromTemplates() : base(20)
        {
        }

        protected override void Process(CompilePipeline pipeline)
        {
            // todo: consider if imports should be omitted or not
            var templates = pipeline.Project.Templates.ToList();

            foreach (var template in templates)
            {
                if (pipeline.Project.ProjectItems.OfType<Item>().Any(i => i.Uri == template.Uri))
                {
                    continue;
                }

                CreateItems(pipeline.Context, pipeline.Project, template);
            }
        }

        private void CreateItems([NotNull] ICompileContext context, [NotNull] IProject project, [NotNull] Template template)
        {
            var templateItem = context.Factory.Item(project, template.SourceTextNodes.First(), template.Uri.Guid, template.DatabaseName, template.ItemName, template.ItemIdOrPath, Constants.Templates.Template);
            templateItem.IsEmittable = false;
            templateItem.IsImport = template.IsImport;
            templateItem.IconProperty.SetValue(template.IconProperty);
            templateItem.Fields.Add(context.Factory.Field(templateItem, template.BaseTemplatesProperty.SourceTextNode ?? TextNode.Empty, "__Base template", template.BaseTemplates));
            templateItem.Fields.Add(context.Factory.Field(templateItem, template.LongHelpProperty.SourceTextNode ?? TextNode.Empty, "__Long description", template.LongHelp));
            templateItem.Fields.Add(context.Factory.Field(templateItem, template.ShortHelpProperty.SourceTextNode ?? TextNode.Empty, "__Short description", template.ShortHelp));

            foreach (var templateSection in template.Sections)
            {
                var templateSectionItemIdOrPath = template.ItemIdOrPath + "/" + templateSection.SectionName;
                var templateSectionItem = context.Factory.Item(project, templateSection.SourceTextNodes.First(), templateSection.Uri.Guid, template.DatabaseName, templateSection.SectionName, templateSectionItemIdOrPath, Constants.Templates.TemplateSection);
                templateSectionItem.IsEmittable = false;
                templateSectionItem.IsImport = template.IsImport;
                templateSectionItem.IconProperty.SetValue(templateSection.IconProperty);

                foreach (var templateField in templateSection.Fields)
                {
                    var templateFieldItemIdOrPath = templateSectionItemIdOrPath + "/" + templateField.FieldName;
                    var templateFieldItem = context.Factory.Item(project, templateField.SourceTextNodes.First(), templateField.Uri.Guid, template.DatabaseName, templateField.FieldName, templateFieldItemIdOrPath, Constants.Templates.TemplateField);
                    templateFieldItem.IsEmittable = false;
                    templateFieldItem.IsImport = template.IsImport;

                    templateFieldItem.Fields.Add(context.Factory.Field(templateFieldItem, templateField.LongHelpProperty.SourceTextNode ?? TextNode.Empty, "__Long description", templateField.LongHelp));
                    templateFieldItem.Fields.Add(context.Factory.Field(templateFieldItem, templateField.ShortHelpProperty.SourceTextNode ?? TextNode.Empty, "__Short description", templateField.ShortHelp));
                    templateFieldItem.Fields.Add(context.Factory.Field(templateFieldItem, TextNode.Empty, "Shared", templateField.Shared ? "True" : "False"));
                    templateFieldItem.Fields.Add(context.Factory.Field(templateFieldItem, TextNode.Empty, "Unversioned", templateField.Unversioned ? "True" : "False"));
                    templateFieldItem.Fields.Add(context.Factory.Field(templateFieldItem, templateField.SourceProperty.SourceTextNode ?? TextNode.Empty, "Source", templateField.Source));
                    templateFieldItem.Fields.Add(context.Factory.Field(templateFieldItem, templateField.SortOrderProperty.SourceTextNode ?? TextNode.Empty, "__Sortorder", templateField.SortOrder.ToString()));
                    templateFieldItem.Fields.Add(context.Factory.Field(templateFieldItem, templateField.TypeProperty.SourceTextNode ?? TextNode.Empty, "Type", templateField.Type));

                    project.AddOrMerge(templateFieldItem);
                }

                project.AddOrMerge(templateSectionItem);
            }

            project.AddOrMerge(templateItem);
        }
    }
}
