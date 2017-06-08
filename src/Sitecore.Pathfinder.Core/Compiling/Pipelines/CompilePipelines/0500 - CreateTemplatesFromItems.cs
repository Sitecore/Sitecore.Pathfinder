// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    [Export(typeof(IPipelineProcessor)), Shared]
    public class CreateTemplatesFromItems : PipelineProcessorBase<CompilePipeline>
    {
        public const int CreateTemplatesFromItemsPriority = 500;

        [ImportingConstructor]
        public CreateTemplatesFromItems([NotNull] IFactory factory) : base(CreateTemplatesFromItemsPriority)
        {
            Factory = factory;
        }

        [NotNull]
        protected IFactory Factory { get; }

        protected virtual void CreateTemplate([NotNull] ICompileContext context, [NotNull] IProject project, [NotNull] Item templateItem)
        {
            var template = Factory.Template(templateItem.Database, templateItem.Uri.Guid, templateItem.ItemName, templateItem.ItemIdOrPath).With(templateItem.SourceTextNode, false, templateItem.IsImport);
            template.IsSynthetic = true;

            var baseTemplateField = templateItem.Fields.FirstOrDefault(f => f.FieldName == Constants.FieldNames.BaseTemplate);
            if (baseTemplateField != null)
            {
                template.BaseTemplatesProperty.SetValue(baseTemplateField.ValueProperty);
            }

            var iconField = templateItem.Fields.FirstOrDefault(f => f.FieldName == Constants.FieldNames.Icon);
            if (iconField != null)
            {
                template.IconProperty.SetValue(iconField.ValueProperty);
            }

            foreach (var sectionItem in templateItem.Children)
            {
                var templateSection = Factory.TemplateSection(template, sectionItem.Uri.Guid).With(sectionItem.SourceTextNode);
                templateSection.IsSynthetic = true;
                template.Sections.Add(templateSection);
                templateSection.SectionNameProperty.SetValue(sectionItem.ItemNameProperty);

                var sectionIconField = sectionItem.Fields.FirstOrDefault(f => f.FieldName == Constants.FieldNames.Icon);
                if (sectionIconField != null)
                {
                    templateSection.IconProperty.SetValue(sectionIconField.ValueProperty);
                }

                var sectionSortorderField = sectionItem.Fields.FirstOrDefault(f => f.FieldName == Constants.FieldNames.SortOrder);
                if (sectionSortorderField != null)
                {
                    if (int.TryParse(sectionSortorderField.Value, out var sortorder))
                    {
                        templateSection.SortorderProperty.SetValue(sortorder);
                    }
                }

                foreach (var fieldItem in sectionItem.Children)
                {
                    var templateField = Factory.TemplateField(template, fieldItem.Uri.Guid).With(fieldItem.SourceTextNode);
                    templateField.IsSynthetic = true;
                    templateSection.Fields.Add(templateField);
                    templateField.FieldNameProperty.SetValue(fieldItem.ItemNameProperty);

                    var typeField = fieldItem.Fields.FirstOrDefault(f => f.FieldName == Constants.FieldNames.Type);
                    if (typeField != null)
                    {
                        templateField.TypeProperty.SetValue(typeField.ValueProperty);
                    }
                }
            }

            project.AddOrMerge(template);
        }

        protected override void Process(CompilePipeline pipeline)
        {
            // todo: consider if imports should be omitted
            var templateItems = pipeline.Context.Project.ProjectItems.OfType<Item>().Where(i => i.TemplateIdOrPath == Constants.Templates.TemplateId || string.Equals(i.TemplateIdOrPath, Constants.Templates.TemplatePathId, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var templateItem in templateItems)
            {
                if (pipeline.Context.Project.Indexes.FindQualifiedItem<Template>(templateItem.Uri) == null)
                {
                    CreateTemplate(pipeline.Context, pipeline.Context.Project, templateItem);
                }
            }
        }
    }
}
