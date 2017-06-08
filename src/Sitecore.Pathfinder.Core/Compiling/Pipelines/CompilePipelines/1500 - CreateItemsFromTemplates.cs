// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    // must come after CompileProjectItems as CreateTemplateFromFields may create a new template
    [Export(typeof(IPipelineProcessor)), Shared]
    public class CreateItemsFromTemplates : PipelineProcessorBase<CompilePipeline>
    {
        [NotNull]
        protected IFactory Factory { get; }

        [ImportingConstructor]
        public CreateItemsFromTemplates([NotNull] IFactory factory) : base(1500)
        {
            Factory = factory;
        }

        protected virtual void CreateItems([NotNull] ICompileContext context, [NotNull] IProject project, [NotNull] Template template)
        {
            var item = Factory.Item(template.Database, template.Uri.Guid, template.ItemName, template.ItemIdOrPath, Constants.Templates.TemplateId).With(template.SourceTextNode);
            item.IsEmittable = false;
            item.IsImport = template.IsImport;
            item.IsSynthetic = true;
            item.IconProperty.SetValue(template.IconProperty);
            item.Fields.Add(Factory.Field(item, Constants.FieldNames.BaseTemplate, template.BaseTemplates).With(template.BaseTemplatesProperty.SourceTextNode));

            if (!string.IsNullOrEmpty(template.LongHelp))
            {
                var field = Factory.Field(item, Constants.FieldNames.LongDescription, template.LongHelp).With(template.LongHelpProperty.SourceTextNode);
                field.Language = context.Project.Context.Language;
                field.Version = Version.Latest;
                item.Fields.Add(field);
            }

            if (!string.IsNullOrEmpty(template.ShortHelp))
            {
                var field = Factory.Field(item, Constants.FieldNames.ShortDescription, template.ShortHelp).With(template.ShortHelpProperty.SourceTextNode);
                field.Language = context.Project.Context.Language;
                field.Version = Version.Latest;
                item.Fields.Add(field);
            }

            ((ISourcePropertyBag)item).NewSourceProperty("__origin", item.Uri);
            ((ISourcePropertyBag)item).NewSourceProperty("__origin_reason", nameof(CreateItemsFromTemplates));

            foreach (var templateSection in template.Sections)
            {
                var templateSectionItemIdOrPath = template.ItemIdOrPath + "/" + templateSection.SectionName;
                var templateSectionItem = Factory.Item(template.Database, templateSection.Uri.Guid, templateSection.SectionName, templateSectionItemIdOrPath, Constants.Templates.TemplateSection.Format()).With(templateSection.SourceTextNode);
                templateSectionItem.IsEmittable = false;
                templateSectionItem.IsImport = template.IsImport;
                templateSectionItem.IsSynthetic = true;
                templateSectionItem.IconProperty.SetValue(templateSection.IconProperty);
                ((ISourcePropertyBag)templateSectionItem).NewSourceProperty("__origin", item.Uri);
                ((ISourcePropertyBag)templateSectionItem).NewSourceProperty("__origin_reason", nameof(CreateItemsFromTemplates));

                foreach (var templateField in templateSection.Fields)
                {
                    var templateFieldItemIdOrPath = templateSectionItemIdOrPath + "/" + templateField.FieldName;
                    var templateFieldItem = Factory.Item(template.Database, templateField.Uri.Guid, templateField.FieldName, templateFieldItemIdOrPath, Constants.Templates.TemplateFieldId).With(templateField.SourceTextNode);
                    templateFieldItem.IsEmittable = false;
                    templateFieldItem.IsImport = template.IsImport;
                    templateFieldItem.IsSynthetic = true;

                    if (!string.IsNullOrEmpty(templateField.LongHelp))
                    {
                        var field = Factory.Field(templateFieldItem, Constants.FieldNames.LongDescription, templateField.LongHelp).With(templateField.LongHelpProperty.SourceTextNode);
                        field.Language = context.Project.Context.Language;
                        field.Version = Version.Latest;
                        templateFieldItem.Fields.Add(field);
                    }

                    if (!string.IsNullOrEmpty(templateField.ShortHelp))
                    {
                        var field = Factory.Field(templateFieldItem, Constants.FieldNames.ShortDescription, templateField.ShortHelp).With(templateField.ShortHelpProperty.SourceTextNode);
                        field.Language = context.Project.Context.Language;
                        field.Version = Version.Latest;
                        templateFieldItem.Fields.Add(field);
                    }

                    templateFieldItem.Fields.Add(Factory.Field(templateFieldItem, Constants.FieldNames.Shared, templateField.Shared ? "True" : "False"));
                    templateFieldItem.Fields.Add(Factory.Field(templateFieldItem, Constants.FieldNames.Unversioned, templateField.Unversioned ? "True" : "False"));
                    templateFieldItem.Fields.Add(Factory.Field(templateFieldItem, Constants.FieldNames.Source, templateField.Source).With(templateField.SourceProperty.SourceTextNode));
                    templateFieldItem.Fields.Add(Factory.Field(templateFieldItem, Constants.FieldNames.SortOrder, templateField.Sortorder.ToString()).With(templateField.SortorderProperty.SourceTextNode));
                    templateFieldItem.Fields.Add(Factory.Field(templateFieldItem, Constants.FieldNames.Type, templateField.Type).With(templateField.TypeProperty.SourceTextNode));
                    ((ISourcePropertyBag)templateFieldItem).NewSourceProperty("__origin", item.Uri);
                    ((ISourcePropertyBag)templateFieldItem).NewSourceProperty("__origin_reason", nameof(CreateItemsFromTemplates));

                    project.AddOrMerge(templateFieldItem);
                }

                project.AddOrMerge(templateSectionItem);
            }

            project.AddOrMerge(item);
        }

        protected override void Process(CompilePipeline pipeline)
        {
            // todo: consider if imports should be omitted or not
            var templates = pipeline.Context.Project.Templates.ToList();

            foreach (var template in templates)
            {
                if (pipeline.Context.Project.Indexes.FindQualifiedItem<Item>(template.Uri) == null)
                {
                    CreateItems(pipeline.Context, pipeline.Context.Project, template);
                }
            }
        }
    }
}
