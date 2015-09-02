// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

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
            var itemNameTextNode = GetItemNameTextNode(context.ParseContext, textNode);
            var itemIdOrPath = context.ParentItemPath + "/" + itemNameTextNode.Value;
            var projectUniqueId = textNode.GetAttributeValue("Id", itemIdOrPath);

            var template = context.ParseContext.Factory.Template(context.ParseContext.Project, projectUniqueId, textNode, context.ParseContext.DatabaseName, itemNameTextNode.Value, itemIdOrPath);
            template.ItemNameProperty.AddSourceTextNode(itemNameTextNode);
            template.BaseTemplatesProperty.Parse(textNode, Constants.Templates.StandardTemplate);
            template.IconProperty.Parse(textNode);
            template.ShortHelpProperty.Parse(textNode);
            template.LongHelpProperty.Parse(textNode);

            template.References.AddRange(ParseReferences(context, template, textNode, template.BaseTemplates));

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
                field.FieldNameProperty.SetValue("__Renderings");
                field.ValueProperty.SetValue(htmlTemplate.Value);
                field.ValueHintProperty.SetValue("HtmlTemplate");
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

            var templateField = templateSection.Fields.FirstOrDefault(f => string.Compare(f.FieldName, fieldName.Value, StringComparison.OrdinalIgnoreCase) == 0);
            if (templateField == null)
            {
                templateField = context.ParseContext.Factory.TemplateField(template);
                templateSection.Fields.Add(templateField);
                templateField.FieldNameProperty.SetValue(fieldName);
            }

            templateField.TypeProperty.Parse(fieldTextNode, "Single-Line Text");
            templateField.Shared = string.Compare(fieldTextNode.GetAttributeValue("Sharing"), "Shared", StringComparison.OrdinalIgnoreCase) == 0;
            templateField.Unversioned = string.Compare(fieldTextNode.GetAttributeValue("Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase) == 0;
            templateField.SourceProperty.Parse(fieldTextNode);
            templateField.ShortHelpProperty.Parse(fieldTextNode);
            templateField.LongHelpProperty.Parse(fieldTextNode);
            templateField.SortOrderProperty.Parse(fieldTextNode, nextSortOrder);

            nextSortOrder = templateField.SortOrder + 100;

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
                    field.FieldNameProperty.SetValue(fieldName);
                    field.ValueProperty.SetValue(standardValue);

                    template.StandardValuesItem.Fields.Add(field);
                }
            }

            template.References.AddRange(ParseReferences(context, template, fieldTextNode, templateField.Source));
        }

        protected virtual void ParseSection([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] ITextNode templateSectionTextNode)
        {
            var sectionName = templateSectionTextNode.GetAttributeTextNode("Name");
            if (sectionName == null)
            {
                context.ParseContext.Trace.TraceError(Texts._Section__element_must_have_a__Name__attribute, templateSectionTextNode);
                return;
            }

            var templateSection = template.Sections.FirstOrDefault(s => string.Compare(s.SectionName, sectionName.Value, StringComparison.OrdinalIgnoreCase) == 0);
            if (templateSection == null)
            {
                templateSection = context.ParseContext.Factory.TemplateSection(templateSectionTextNode);
                templateSection.SectionNameProperty.SetValue(sectionName);

                template.Sections.Add(templateSection);
            }

            templateSection.IconProperty.Parse(templateSectionTextNode);

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
