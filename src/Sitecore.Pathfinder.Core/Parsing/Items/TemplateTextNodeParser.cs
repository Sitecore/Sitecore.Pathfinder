// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing.Pipelines.TemplateParserPipelines;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Parsing.Items
{
    public class TemplateTextNodeParser : TextNodeParserBase
    {
        [ImportingConstructor]
        public TemplateTextNodeParser([NotNull] ISchemaService schemaService) : base(Constants.TextNodeParsers.Templates)
        {
            SchemaService = schemaService;
        }

        [NotNull]
        protected ISchemaService SchemaService { get; }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Key == "Template";
        }

        public override void Parse(ItemParseContext context, ITextNode textNode)
        {
            var itemNameTextNode = GetItemNameTextNode(context.ParseContext, textNode);
            var parentItemPath = textNode.GetAttributeValue("ParentItemPath", context.ParentItemPath);
            var itemIdOrPath = textNode.GetAttributeValue("ItemPath");
            if (string.IsNullOrEmpty(itemIdOrPath))
            {
                itemIdOrPath = PathHelper.CombineItemPath(parentItemPath, itemNameTextNode.Value);
            }
            else if (itemNameTextNode.Value != Path.GetFileName(itemIdOrPath))
            {
                context.ParseContext.Trace.TraceError(Msg.P1000, "Item name in 'ItemPath' and 'Name' does not match. Using 'Name'");
            }

            var guid = StringHelper.GetGuid(context.ParseContext.Project, textNode.GetAttributeValue("Id", itemIdOrPath));
            var databaseName = textNode.GetAttributeValue("Database", context.DatabaseName);

            var template = context.ParseContext.Factory.Template(context.ParseContext.Project, guid, databaseName, itemNameTextNode.Value, itemIdOrPath).With(textNode);
            template.ItemNameProperty.AddSourceTextNode(itemNameTextNode);
            template.BaseTemplatesProperty.Parse(textNode, Constants.Templates.StandardTemplateId);
            template.IconProperty.Parse(textNode);
            template.ShortHelpProperty.Parse(textNode);
            template.LongHelpProperty.Parse(textNode);

            // todo: yuck
            template.IsEmittable = !string.Equals(textNode.GetAttributeValue(Constants.Fields.IsEmittable), "False", StringComparison.OrdinalIgnoreCase);
            template.IsImport = string.Equals(textNode.GetAttributeValue(Constants.Fields.IsImport, context.IsImport.ToString()), "True", StringComparison.OrdinalIgnoreCase);

            template.References.AddRange(context.ParseContext.ReferenceParser.ParseReferences(template, template.BaseTemplatesProperty));

            // create standard values item
            var standardValuesItemIdOrPath = itemIdOrPath + "/__Standard Values";
            var standardValuesGuid = StringHelper.GetGuid(context.ParseContext.Project, standardValuesItemIdOrPath);
            var standardValuesItem = context.ParseContext.Factory.Item(context.ParseContext.Project, standardValuesGuid, databaseName, "__Standard Values", standardValuesItemIdOrPath, itemIdOrPath).With(textNode);
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

            context.ParseContext.Pipelines.Resolve<TemplateParserPipeline>().Execute(context, template, textNode);

            context.ParseContext.Project.AddOrMerge(template);
            context.ParseContext.Project.AddOrMerge(standardValuesItem);
        }

        protected virtual void ParseField([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] TemplateSection templateSection, [NotNull] ITextNode templateFieldTextNode, ref int nextSortOrder)
        {
            SchemaService.ValidateTextNodeSchema(templateFieldTextNode, "TemplateField");

            var fieldName = templateFieldTextNode.GetAttribute("Name");
            if (fieldName == null)
            {
                context.ParseContext.Trace.TraceError(Msg.P1005, Texts._Field__element_must_have_a__Name__attribute, templateFieldTextNode.Snapshot.SourceFile.AbsoluteFileName, templateFieldTextNode.TextSpan);
                return;
            }

            var templateField = templateSection.Fields.FirstOrDefault(f => string.Equals(f.FieldName, fieldName.Value, StringComparison.OrdinalIgnoreCase));
            if (templateField == null)
            {
                var itemIdOrPath = template.ItemIdOrPath + "/" + templateSection.SectionName + "/" + fieldName.Value;
                var guid = StringHelper.GetGuid(template.Project, templateFieldTextNode.GetAttributeValue("Id", itemIdOrPath));

                templateField = context.ParseContext.Factory.TemplateField(template, guid).With(templateFieldTextNode);
                templateSection.Fields.Add(templateField);
                templateField.FieldNameProperty.SetValue(fieldName);
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
                    field.FieldNameProperty.SetValue(fieldName);
                    field.ValueProperty.SetValue(standardValueTextNode);

                    template.StandardValuesItem.Fields.Add(field);
                }
            }

            template.References.AddRange(context.ParseContext.ReferenceParser.ParseReferences(template, templateField.SourceProperty));
        }

        protected virtual void ParseSection([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] ITextNode templateSectionTextNode)
        {
            SchemaService.ValidateTextNodeSchema(templateSectionTextNode, "TemplateSection");

            var sectionName = templateSectionTextNode.GetAttribute("Name");
            if (sectionName == null)
            {
                context.ParseContext.Trace.TraceError(Msg.P1007, Texts._Section__element_must_have_a__Name__attribute, templateSectionTextNode);
                return;
            }

            var templateSection = template.Sections.FirstOrDefault(s => string.Equals(s.SectionName, sectionName.Value, StringComparison.OrdinalIgnoreCase));
            if (templateSection == null)
            {
                var itemIdOrPath = template.ItemIdOrPath + "/" + sectionName.Value;
                var guid = StringHelper.GetGuid(template.Project, templateSectionTextNode.GetAttributeValue("Id", itemIdOrPath));

                templateSection = context.ParseContext.Factory.TemplateSection(template, guid).With(templateSectionTextNode);
                templateSection.SectionNameProperty.SetValue(sectionName);

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
