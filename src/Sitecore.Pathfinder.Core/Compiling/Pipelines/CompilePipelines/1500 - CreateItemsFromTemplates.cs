// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Compiling.Compilers;
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
        public CreateItemsFromTemplates() : base(1500)
        {
        }

        protected override void Process(CompilePipeline pipeline)
        {
            // todo: consider if imports should be omitted or not
            var templates = pipeline.Context.Project.Templates.ToList();

            foreach (var template in templates)
            {
                if (pipeline.Context.Project.FindQualifiedItem<Item>(template.Uri) == null)
                {
                    CreateItems(pipeline.Context, pipeline.Context.Project, template);
                }
            }
        }

        protected virtual void CreateItems([NotNull] ICompileContext context, [NotNull] IProject project, [NotNull] Template template)
        {
            var item = context.Factory.Item(project, template.Uri.Guid, template.DatabaseName, template.ItemName, template.ItemIdOrPath, Constants.Templates.TemplateId).With(template.SourceTextNode);
            item.IsEmittable = false;
            item.IsImport = template.IsImport;
            item.IconProperty.SetValue(template.IconProperty);
            item.Fields.Add(context.Factory.Field(item, "__Base template", template.BaseTemplates).With(template.BaseTemplatesProperty.SourceTextNode));
            item.Fields.Add(context.Factory.Field(item, "__Long description", template.LongHelp).With(template.LongHelpProperty.SourceTextNode));
            item.Fields.Add(context.Factory.Field(item, "__Short description", template.ShortHelp).With(template.ShortHelpProperty.SourceTextNode));
            ((ISourcePropertyBag)item).NewSourceProperty("__origin", item.Uri);
            ((ISourcePropertyBag)item).NewSourceProperty("__origin_reason", nameof(CreateItemsFromTemplates));

            foreach (var templateSection in template.Sections)
            {
                var templateSectionItemIdOrPath = template.ItemIdOrPath + "/" + templateSection.SectionName;
                var templateSectionItem = context.Factory.Item(project, templateSection.Uri.Guid, template.DatabaseName, templateSection.SectionName, templateSectionItemIdOrPath, Constants.Templates.TemplateSection.Format()).With(templateSection.SourceTextNode);
                templateSectionItem.IsEmittable = false;
                templateSectionItem.IsImport = template.IsImport;
                templateSectionItem.IconProperty.SetValue(templateSection.IconProperty);
                ((ISourcePropertyBag)templateSectionItem).NewSourceProperty("__origin", item.Uri);
                ((ISourcePropertyBag)templateSectionItem).NewSourceProperty("__origin_reason", nameof(CreateItemsFromTemplates));

                foreach (var templateField in templateSection.Fields)
                {
                    var templateFieldItemIdOrPath = templateSectionItemIdOrPath + "/" + templateField.FieldName;
                    var templateFieldItem = context.Factory.Item(project, templateField.Uri.Guid, template.DatabaseName, templateField.FieldName, templateFieldItemIdOrPath, Constants.Templates.TemplateFieldId).With(templateField.SourceTextNode);
                    templateFieldItem.IsEmittable = false;
                    templateFieldItem.IsImport = template.IsImport;

                    templateFieldItem.Fields.Add(context.Factory.Field(templateFieldItem, "__Long description", templateField.LongHelp).With(templateField.LongHelpProperty.SourceTextNode));
                    templateFieldItem.Fields.Add(context.Factory.Field(templateFieldItem, "__Short description", templateField.ShortHelp).With(templateField.ShortHelpProperty.SourceTextNode));
                    templateFieldItem.Fields.Add(context.Factory.Field(templateFieldItem, "Shared", templateField.Shared ? "True" : "False"));
                    templateFieldItem.Fields.Add(context.Factory.Field(templateFieldItem, "Unversioned", templateField.Unversioned ? "True" : "False"));
                    templateFieldItem.Fields.Add(context.Factory.Field(templateFieldItem, "Source", templateField.Source).With(templateField.SourceProperty.SourceTextNode));
                    templateFieldItem.Fields.Add(context.Factory.Field(templateFieldItem, "__Sortorder", templateField.Sortorder.ToString()).With(templateField.SortorderProperty.SourceTextNode));
                    templateFieldItem.Fields.Add(context.Factory.Field(templateFieldItem, "Type", templateField.Type).With(templateField.TypeProperty.SourceTextNode));
                    ((ISourcePropertyBag)templateFieldItem).NewSourceProperty("__origin", item.Uri);
                    ((ISourcePropertyBag)templateFieldItem).NewSourceProperty("__origin_reason", nameof(CreateItemsFromTemplates));

                    project.AddOrMerge(templateFieldItem);
                }

                project.AddOrMerge(templateSectionItem);
            }

            project.AddOrMerge(item);
        }
    }
}
