// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing.Pipelines.TemplateParserPipelines;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Parsing.Items
{
    public abstract class TemplateTextNodeParserBase : TextNodeParserBase
    {
        [ImportingConstructor]
        protected TemplateTextNodeParserBase([NotNull] IPipelineService pipelines, [NotNull] ISchemaService schemaService, double priority) : base(priority)
        {
            Pipelines = pipelines;
            SchemaService = schemaService;
        }

        [NotNull]
        protected IPipelineService Pipelines { get; }

        [NotNull]
        protected ISchemaService SchemaService { get; }

        public override void Parse(ItemParseContext context, ITextNode textNode)
        {
            SchemaService.ValidateTextNodeSchema(textNode, "Template");

            var itemNameTextNode = GetItemNameTextNode(context.ParseContext, textNode);
            var parentItemPath = textNode.GetAttributeValue("ParentItemPath", context.ParentItemPath);
            var itemIdOrPath = textNode.GetAttributeValue("ItemPath");
            if (string.IsNullOrEmpty(itemIdOrPath))
            {
                itemIdOrPath = PathHelper.CombineItemPath(parentItemPath, itemNameTextNode.Value);
            }
            else if (itemNameTextNode.Value != Path.GetFileName(itemIdOrPath))
            {
                context.ParseContext.Trace.TraceError(Msg.P1034, "Item name in 'ItemPath' and 'Name' does not match. Using 'Name'");
            }

            var guid = StringHelper.GetGuid(context.ParseContext.Project, textNode.GetAttributeValue("Id", itemIdOrPath));
            var databaseName = textNode.GetAttributeValue("Database", context.Database.DatabaseName);
            var database = context.ParseContext.Project.GetDatabase(databaseName);

            var template = context.ParseContext.Factory.Template(database, guid, itemNameTextNode.Value, itemIdOrPath).With(textNode);
            template.ItemNameProperty.AddSourceTextNode(itemNameTextNode);
            template.BaseTemplatesProperty.Parse(textNode, Constants.Templates.StandardTemplateId);
            template.IconProperty.Parse(textNode);
            template.ShortHelpProperty.Parse(textNode);
            template.LongHelpProperty.Parse(textNode);

            template.IsEmittable = textNode.GetAttributeBool(Constants.Fields.IsEmittable, true);
            template.IsImport = textNode.GetAttributeBool(Constants.Fields.IsImport, context.IsImport);

            template.References.AddRange(context.ParseContext.ReferenceParser.ParseReferences(template, template.BaseTemplatesProperty));

            // create standard values item
            var standardValuesItemIdOrPath = itemIdOrPath + "/__Standard Values";
            var standardValuesGuid = StringHelper.GetGuid(context.ParseContext.Project, standardValuesItemIdOrPath);
            var standardValuesItem = context.ParseContext.Factory.Item(database, standardValuesGuid, "__Standard Values", standardValuesItemIdOrPath, itemIdOrPath).With(textNode);
            standardValuesItem.IsImport = template.IsImport;

            // todo: should be Uri
            template.StandardValuesItem = standardValuesItem;

            // parse fields and sections
            var sections = textNode.GetSnapshotLanguageSpecificChildNode("Sections");
            if (sections != null)
            {
                foreach (var sectionTreeNode in sections.ChildNodes)
                {
                    ParseSection(context, template, sectionTreeNode);
                }
            }

            Pipelines.Resolve<TemplateParserPipeline>().Execute(context, template, textNode);

            context.ParseContext.Project.AddOrMerge(template);
            context.ParseContext.Project.AddOrMerge(standardValuesItem);
        }

        protected virtual void ParseField([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] TemplateSection templateSection, [NotNull] ITextNode templateFieldTextNode, ref int nextSortOrder)
        {
            SchemaService.ValidateTextNodeSchema(templateFieldTextNode, "TemplateField");

            GetName(context.ParseContext, templateFieldTextNode, out string fieldName, out ITextNode fieldNameTextNode, "Field", "Name");
            if (string.IsNullOrEmpty(fieldName))
            {
                context.ParseContext.Trace.TraceError(Msg.P1005, Texts._Field__element_must_have_a__Name__attribute, templateFieldTextNode.Snapshot.SourceFile.AbsoluteFileName, templateFieldTextNode.TextSpan);
                return;
            }

            var templateField = templateSection.Fields.FirstOrDefault(f => string.Equals(f.FieldName, fieldName, StringComparison.OrdinalIgnoreCase));
            if (templateField == null)
            {
                var itemIdOrPath = template.ItemIdOrPath + "/" + templateSection.SectionName + "/" + fieldName;
                var guid = StringHelper.GetGuid(template.Project, templateFieldTextNode.GetAttributeValue("Id", itemIdOrPath));

                templateField = context.ParseContext.Factory.TemplateField(template, guid).With(templateFieldTextNode);
                templateSection.Fields.Add(templateField);
                templateField.FieldNameProperty.SetValue(fieldNameTextNode);
                templateField.FieldName = fieldName;
            }

            templateField.TypeProperty.Parse(templateFieldTextNode, "Single-Line Text");
            templateField.Shared = string.Equals(templateFieldTextNode.GetAttributeValue("Sharing"), "Shared", StringComparison.OrdinalIgnoreCase);
            templateField.Unversioned = string.Equals(templateFieldTextNode.GetAttributeValue("Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase);
            templateField.SourceProperty.Parse(templateFieldTextNode);
            templateField.ShortHelpProperty.Parse(templateFieldTextNode);
            templateField.LongHelpProperty.Parse(templateFieldTextNode);
            templateField.SortorderProperty.Parse(templateFieldTextNode, nextSortOrder);

            nextSortOrder = templateField.Sortorder + 100;

            var standardValueTextNode = templateFieldTextNode.GetAttribute("StandardValue");
            if (standardValueTextNode != null && !string.IsNullOrEmpty(standardValueTextNode.Value))
            {
                if (template.StandardValuesItem == null)
                {
                    context.ParseContext.Trace.TraceError(Msg.P1006, Texts.Template_does_not_a_standard_values_item, standardValueTextNode);
                }
                else
                {
                    // todo: support language and version
                    var field = context.ParseContext.Factory.Field(template.StandardValuesItem).With(standardValueTextNode);
                    field.FieldNameProperty.SetValue(fieldNameTextNode);
                    field.ValueProperty.SetValue(standardValueTextNode);

                    template.StandardValuesItem.Fields.Add(field);
                }
            }

            template.References.AddRange(context.ParseContext.ReferenceParser.ParseReferences(template, templateField.SourceProperty));
        }

        protected virtual void ParseSection([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] ITextNode templateSectionTextNode)
        {
            SchemaService.ValidateTextNodeSchema(templateSectionTextNode, "TemplateSection");

            GetName(context.ParseContext, templateSectionTextNode, out string sectionName, out ITextNode sectionNameTextNode, "Section", "Name");
            if (string.IsNullOrEmpty(sectionName))
            {
                context.ParseContext.Trace.TraceError(Msg.P1007, Texts._Section__element_must_have_a__Name__attribute, sectionNameTextNode);
                return;
            }

            var templateSection = template.Sections.FirstOrDefault(s => string.Equals(s.SectionName, sectionName, StringComparison.OrdinalIgnoreCase));
            if (templateSection == null)
            {
                var itemIdOrPath = template.ItemIdOrPath + "/" + sectionName;
                var guid = StringHelper.GetGuid(template.Project, templateSectionTextNode.GetAttributeValue("Id", itemIdOrPath));

                templateSection = context.ParseContext.Factory.TemplateSection(template, guid).With(templateSectionTextNode);
                templateSection.SectionNameProperty.SetValue(sectionNameTextNode);
                templateSection.SectionName = sectionName;

                template.Sections.Add(templateSection);
            }

            templateSection.IconProperty.Parse(templateSectionTextNode);

            var fieldsTextNode = templateSectionTextNode.GetSnapshotLanguageSpecificChildNode("Fields");
            if (fieldsTextNode == null)
            {
                return;
            }

            var nextSortOrder = 0;
            foreach (var fieldTextNode in fieldsTextNode.ChildNodes)
            {
                ParseField(context, template, templateSection, fieldTextNode, ref nextSortOrder);
            }
        }
    }
}
