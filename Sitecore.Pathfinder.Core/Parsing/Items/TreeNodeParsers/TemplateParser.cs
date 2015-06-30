// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
    [Export(typeof(ITextNodeParser))]
    public class TemplateParser : TextNodeParserBase
    {
        public TemplateParser() : base(Constants.TextNodeParsers.Templates)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Name == "Template";
        }

        public override void Parse(ItemParseContext context, ITextNode textNode)
        {
            var itemName = GetItemName(context.ParseContext, textNode);
            var itemIdOrPath = context.ParentItemPath + "/" + itemName.Value;
            var projectUniqueId = textNode.GetAttributeValue("Id", itemIdOrPath);

            var template = context.ParseContext.Factory.Template(context.ParseContext.Project, projectUniqueId, textNode, context.ParseContext.DatabaseName, itemName, itemIdOrPath);
            template.ItemName.Merge(itemName);
            template.BaseTemplates.Parse(textNode, Constants.Templates.StandardTemplate);
            template.Icon.Parse(textNode);
            template.ShortHelp.Parse(textNode);
            template.LongHelp.Parse(textNode);

            template.References.AddRange(ParseReferences(context, template, textNode, template.BaseTemplates.Value));

            // create standard values item
            var standardValuesItemIdOrPath = itemIdOrPath + "/__Standard Values";
            var standardValuesItem = context.ParseContext.Factory.Item(context.ParseContext.Project, standardValuesItemIdOrPath, textNode, context.ParseContext.DatabaseName, "__Standard Values", standardValuesItemIdOrPath, itemIdOrPath);

            template.StandardValuesItem = standardValuesItem;

            // parse fields and sections
            var sections = context.Snapshot.GetJsonChildTextNode(textNode, "Sections");
            if (sections != null)
            {
                foreach (var sectionTreeNode in sections.ChildNodes)
                {
                    ParseSection(context, template, sectionTreeNode);
                }
            }

            // setup HtmlTemplate
            var htmlTemplate = textNode.GetAttributeTextNode("Layout.HtmlFile");
            if (htmlTemplate != null && !string.IsNullOrEmpty(htmlTemplate.Value))
            {
                var field = context.ParseContext.Factory.Field(template.StandardValuesItem);
                field.FieldName.SetValue("__Renderings");
                field.Value.SetValue(htmlTemplate.Value);
                field.ValueHint.SetValue("HtmlTemplate");
                template.StandardValuesItem.Fields.Add(field);
            }

            context.ParseContext.Project.AddOrMerge(context.ParseContext, template);
            context.ParseContext.Project.AddOrMerge(context.ParseContext, standardValuesItem);
        }

        protected virtual void ParseField([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] TemplateSection templateSection, [NotNull] ITextNode fieldTextNode, ref int nextSortOrder)
        {
            var fieldName = fieldTextNode.GetAttributeTextNode("Name");
            if (fieldName == null)
            {
                context.ParseContext.Trace.TraceError(Texts._Field__element_must_have_a__Name__attribute, fieldTextNode.Snapshot.SourceFile.FileName, fieldTextNode.Position);
                return;
            }

            var templateField = templateSection.Fields.FirstOrDefault(f => string.Compare(f.FieldName.Value, fieldName.Value, StringComparison.OrdinalIgnoreCase) == 0);
            if (templateField == null)
            {
                templateField = context.ParseContext.Factory.TemplateField(template);
                templateSection.Fields.Add(templateField);
                templateField.FieldName.AddSource(fieldName);
            }

            templateField.Type.Parse(fieldTextNode, "Single-Line Text");
            templateField.Shared = string.Compare(fieldTextNode.GetAttributeValue("Sharing"), "Shared", StringComparison.OrdinalIgnoreCase) == 0;
            templateField.Unversioned = string.Compare(fieldTextNode.GetAttributeValue("Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase) == 0;
            templateField.Source.Parse(fieldTextNode);
            templateField.ShortHelp.Parse(fieldTextNode);
            templateField.LongHelp.Parse(fieldTextNode);
            templateField.SortOrder.Parse(fieldTextNode, nextSortOrder);

            nextSortOrder = templateField.SortOrder.Value + 100;

            var standardValue = fieldTextNode.GetAttributeTextNode("StandardValue");
            if (standardValue != null && !string.IsNullOrEmpty(standardValue.Value))
            {
                if (template.StandardValuesItem == null)
                {
                    context.ParseContext.Trace.TraceError(Texts.Template_does_not_a_standard_values_item, standardValue);
                }
                else
                {
                    // todo: support language and version
                    var field = context.ParseContext.Factory.Field(template.StandardValuesItem);
                    field.FieldName.AddSource(fieldName);
                    field.Value.AddSource(standardValue);

                    template.StandardValuesItem.Fields.Add(field);
                }
            }

            template.References.AddRange(ParseReferences(context, template, fieldTextNode, templateField.Source.Value));
        }

        protected virtual void ParseSection([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] ITextNode templateSectionTextNode)
        {
            var sectionName = templateSectionTextNode.GetAttributeTextNode("Name");
            if (sectionName == null)
            {
                context.ParseContext.Trace.TraceError(Texts._Section__element_must_have_a__Name__attribute, templateSectionTextNode);
                return;
            }

            var templateSection = template.Sections.FirstOrDefault(s => string.Compare(s.SectionName.Value, sectionName.Value, StringComparison.OrdinalIgnoreCase) == 0);
            if (templateSection == null)
            {
                templateSection = context.ParseContext.Factory.TemplateSection(templateSectionTextNode);
                templateSection.SectionName.AddSource(sectionName);

                template.Sections.Add(templateSection);
            }

            templateSection.Icon.Parse(templateSectionTextNode);

            var fieldsTextNode = context.Snapshot.GetJsonChildTextNode(templateSectionTextNode, "Fields");
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
