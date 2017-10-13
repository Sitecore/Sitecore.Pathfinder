// © 2015-2017 by Jakob Christensen. All rights reserved.

using System;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing.Pipelines.TemplateParserPipelines;
using Sitecore.Pathfinder.Parsing.References;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Parsing.Items
{
    public abstract class TemplateTextNodeParserBase : TextNodeParserBase
    {
        protected TemplateTextNodeParserBase([NotNull] IFactory factory, [NotNull] ITraceService trace, [NotNull] IPipelineService pipelines, [NotNull] IReferenceParserService referenceParser, [NotNull] ISchemaService schemaService, double priority) : base(priority)
        {
            Factory = factory;
            Trace = trace;
            Pipelines = pipelines;
            ReferenceParser = referenceParser;
            SchemaService = schemaService;
        }

        [NotNull]
        public override ISchemaService SchemaService { get; }

        [NotNull]
        protected IFactory Factory { get; }

        [NotNull]
        protected IPipelineService Pipelines { get; }

        [NotNull]
        protected IReferenceParserService ReferenceParser { get; }

        [NotNull]
        protected ITraceService Trace { get; }

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
                Trace.TraceError(Msg.P1034, "Item name in 'ItemPath' and 'Name' does not match. Using 'Name'");
            }

            var guid = StringHelper.GetGuid(context.ParseContext.Project, textNode.GetAttributeValue("Id", itemIdOrPath));
            var databaseName = textNode.GetAttributeValue("Database", context.Database.DatabaseName);
            var database = context.ParseContext.Project.GetDatabase(databaseName);

            var template = Factory.Template(database, guid, itemNameTextNode.Value, itemIdOrPath);
            template.ItemNameProperty.AddSourceTextNode(itemNameTextNode);
            template.BaseTemplatesProperty.Parse(textNode, Constants.Templates.StandardTemplateId);
            template.IconProperty.Parse(textNode);
            template.ShortHelpProperty.Parse(textNode);
            template.LongHelpProperty.Parse(textNode);

            template.IsEmittable = textNode.GetAttributeBool(Constants.Fields.IsEmittable, true);
            template.IsImport = textNode.GetAttributeBool(Constants.Fields.IsImport, context.IsImport);

            template.References.AddRange(ReferenceParser.ParseReferences(template, template.BaseTemplatesProperty));

            Item standardValuesItem;
            var standardValuesTextNode = textNode.ChildNodes.FirstOrDefault(n => string.Equals(n.Value, "__Standard Values", StringComparison.OrdinalIgnoreCase));
            if (standardValuesTextNode != null)
            {
                var newContext = Factory.ItemParseContext(context.ParseContext, context.Parser, template.Database, template.ItemIdOrPath, template.IsImport);
                context.Parser.ParseTextNode(newContext, standardValuesTextNode);
                standardValuesItem = template.Database.GetItem(template.ItemIdOrPath + "/__Standard Values");

                if (standardValuesItem == null)
                {
                    Trace.TraceError(Msg.C1137, "'__Standard Values' item not parsed correctly", standardValuesTextNode);
                }
            }
            else
            {
                // create standard values item
                var standardValuesItemIdOrPath = itemIdOrPath + "/__Standard Values";
                var standardValuesGuid = StringHelper.GetGuid(context.ParseContext.Project, standardValuesItemIdOrPath);
                standardValuesItem = Factory.Item(database, standardValuesGuid, "__Standard Values", standardValuesItemIdOrPath, itemIdOrPath).With(textNode);
                context.ParseContext.Project.AddOrMerge(standardValuesItem);
            }

            if (standardValuesItem != null)
            {
                // todo: should be Uri
                template.StandardValuesItem = standardValuesItem;
                standardValuesItem.IsImport = template.IsImport;
            }

            // parse fields and sections
            foreach (var sectionTreeNode in textNode.ChildNodes)
            {
                ParseSection(context, template, sectionTreeNode);
            }

            Pipelines.GetPipeline<TemplateParserPipeline>().Execute(context, template, textNode);

            context.ParseContext.Project.AddOrMerge(template);
        }

        protected virtual void ParseField([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] TemplateSection templateSection, [NotNull] ITextNode templateFieldTextNode, ref int nextSortOrder)
        {
            SchemaService.ValidateTextNodeSchema(templateFieldTextNode, "TemplateField");

            GetName(context.ParseContext, templateFieldTextNode, out var fieldName, out var fieldNameTextNode, "Field", "Name");
            if (string.IsNullOrEmpty(fieldName))
            {
                Trace.TraceError(Msg.P1005, Texts._Field__element_must_have_a__Name__attribute, templateFieldTextNode.Snapshot.SourceFile.AbsoluteFileName, templateFieldTextNode.TextSpan);
                return;
            }

            var templateField = templateSection.Fields.FirstOrDefault(f => string.Equals(f.FieldName, fieldName, StringComparison.OrdinalIgnoreCase));
            if (templateField == null)
            {
                var itemIdOrPath = template.ItemIdOrPath + "/" + templateSection.SectionName + "/" + fieldName;
                var guid = StringHelper.GetGuid(template.Project, templateFieldTextNode.GetAttributeValue("Id", itemIdOrPath));

                templateField = Factory.TemplateField(template, guid).With(templateFieldTextNode);
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

            // set field standard value
            var standardValueTextNode = templateFieldTextNode.GetAttribute("StandardValue");
            if (standardValueTextNode != null && !string.IsNullOrEmpty(standardValueTextNode.Value))
            {
                if (template.StandardValuesItem == null)
                {
                    Trace.TraceError(Msg.P1006, Texts.Template_does_not_a_standard_values_item, standardValueTextNode);
                }
                else
                {
                    var field = template.StandardValuesItem.Fields.GetField(templateField.FieldName);
                    if (field == null)
                    {
                        field = Factory.Field(template.StandardValuesItem).With(standardValueTextNode);
                        field.FieldNameProperty.SetValue(fieldNameTextNode);
                        template.StandardValuesItem.Fields.Add(field);
                    }

                    field.ValueProperty.SetValue(standardValueTextNode);
                }
            }

            template.References.AddRange(ReferenceParser.ParseReferences(template, templateField.SourceProperty));
        }

        protected virtual void ParseSection([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] ITextNode templateSectionTextNode)
        {
            if (string.Equals(templateSectionTextNode.Value, "__Standard Values", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            SchemaService.ValidateTextNodeSchema(templateSectionTextNode, "TemplateSection");

            GetName(context.ParseContext, templateSectionTextNode, out var sectionName, out var sectionNameTextNode, "Section", "Name");
            if (string.IsNullOrEmpty(sectionName))
            {
                Trace.TraceError(Msg.P1007, Texts._Section__element_must_have_a__Name__attribute, sectionNameTextNode);
                return;
            }

            var templateSection = template.Sections.FirstOrDefault(s => string.Equals(s.SectionName, sectionName, StringComparison.OrdinalIgnoreCase));
            if (templateSection == null)
            {
                var itemIdOrPath = template.ItemIdOrPath + "/" + sectionName;
                var guid = StringHelper.GetGuid(template.Project, templateSectionTextNode.GetAttributeValue("Id", itemIdOrPath));

                templateSection = Factory.TemplateSection(template, guid).With(templateSectionTextNode);
                templateSection.SectionNameProperty.SetValue(sectionNameTextNode);
                templateSection.SectionName = sectionName;

                template.Sections.Add(templateSection);
            }

            templateSection.IconProperty.Parse(templateSectionTextNode);

            var nextSortOrder = 0;
            foreach (var fieldTextNode in templateSectionTextNode.ChildNodes)
            {
                ParseField(context, template, templateSection, fieldTextNode, ref nextSortOrder);
            }
        }
    }
}
