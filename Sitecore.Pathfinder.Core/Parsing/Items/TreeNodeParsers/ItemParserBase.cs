// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Parsing.Pipelines.ItemParserPipelines;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
    public abstract class ItemParserBase : TextNodeParserBase
    {
        protected ItemParserBase(double priority) : base(priority)
        {
        }

        public override void Parse(ItemParseContext context, ITextNode textNode)
        {
            var itemNameTextNode = GetItemNameTextNode(context.ParseContext, textNode);
            var itemIdOrPath = context.ParentItemPath + "/" + itemNameTextNode.Value;
            var projectUniqueId = textNode.GetAttributeValue("Id", itemIdOrPath);
            var templateIdOrPathTextNode = GetTemplateIdOrPathTextNode(context, textNode);
            var templateIdOrPath = templateIdOrPathTextNode?.Value ?? string.Empty;

            var item = context.ParseContext.Factory.Item(context.ParseContext.Project, projectUniqueId, textNode, context.ParseContext.DatabaseName, itemNameTextNode.Value, itemIdOrPath, templateIdOrPath);
            item.ItemNameProperty.AddSourceTextNode(itemNameTextNode);

            if (templateIdOrPathTextNode != null)
            {
                item.TemplateIdOrPathProperty.AddSourceTextNode(templateIdOrPathTextNode);
                item.References.AddRange(ParseReferences(context, item, templateIdOrPathTextNode, templateIdOrPathTextNode.Value));
            }

            context.ParseContext.PipelineService.Resolve<ItemParserPipeline>().Execute(context, item, textNode);

            ParseChildNodes(context, item, textNode);

            context.ParseContext.Project.AddOrMerge(context.ParseContext, item);
        }

        [CanBeNull]
        protected virtual ITextNode GetTemplateIdOrPathTextNode([NotNull] ItemParseContext context, [NotNull] ITextNode textNode)
        {
            // todo: consider moving template into the ItemParserPipeline
            var templateIdOrPath = textNode.GetAttributeTextNode("Template");

            var createTemplate = textNode.GetAttributeValue("Template.CreateFromFields");
            if (string.Compare(createTemplate, "true", StringComparison.OrdinalIgnoreCase) != 0)
            {
                return templateIdOrPath;
            }

            if (templateIdOrPath == null)
            {
                context.ParseContext.Trace.TraceError(Texts.The__Template__attribute_must_be_specified_when__Template_CreateFromFields__equals_true_, textNode);
                return null;
            }

            ParseTemplate(context, textNode, templateIdOrPath);

            return templateIdOrPath;
        }

        protected abstract void ParseChildNodes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode);

        protected virtual void ParseFieldTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode fieldTextNode)
        {
            var fieldName = fieldTextNode.GetAttributeTextNode("Name");
            if (fieldName == null || string.IsNullOrEmpty(fieldName.Value))
            {
                context.ParseContext.Trace.TraceError(Texts._Field__element_must_have_a__Name__attribute, fieldTextNode);
                return;
            }

            // check if field is already defined
            var field = item.Fields.FirstOrDefault(f => string.Compare(f.FieldName, fieldName.Value, StringComparison.OrdinalIgnoreCase) == 0);
            if (field != null)
            {
                context.ParseContext.Trace.TraceError(Texts.Field_is_already_defined, fieldTextNode, fieldName.Value);
            }

            // check if version is an integer
            var versionValue = fieldTextNode.GetAttributeValue("Version");
            if (!string.IsNullOrEmpty(versionValue))
            {
                int version;
                if (!int.TryParse(versionValue, out version))
                {
                    context.ParseContext.Trace.TraceError(Texts._version__attribute_must_have_an_integer_value, fieldTextNode, fieldName.Value);
                }
            }

            field = context.ParseContext.Factory.Field(item);
            field.FieldNameProperty.Parse(fieldTextNode);
            field.LanguageProperty.Parse(fieldTextNode);
            field.VersionProperty.Parse(fieldTextNode);
            field.ValueHintProperty.Parse(fieldTextNode);

            // get field value either from Value attribute or from inner text
            var inner = fieldTextNode.GetInnerTextNode();
            var attribute = fieldTextNode.GetAttributeTextNode("Value");

            if (inner != null && attribute != null)
            {
                context.ParseContext.Trace.TraceWarning(Texts.Value_is_specified_in_both__Value__attribute_and_in_element__Using_value_from_attribute, fieldTextNode.Snapshot.SourceFile.FileName, attribute.Position, fieldName.Value);
                field.ValueProperty.SetValue(attribute);
            }
            else if (inner != null)
            {
                field.ValueProperty.SetValue(inner);
            }
            else if (attribute != null)
            {
                field.ValueProperty.SetValue(attribute);
            }

            item.Fields.Add(field);

            if (field.ValueProperty.SourceTextNode != null && !field.ValueHint.Contains("NoReference"))
            {
                item.References.AddRange(ParseReferences(context, item, field.ValueProperty.SourceTextNode, field.Value));
            }
        }

        [NotNull]
        protected virtual Template ParseTemplate([NotNull] ItemParseContext context, [NotNull] ITextNode itemTextNode, [NotNull] ITextNode templateIdOrPathTextNode)
        {
            var itemIdOrPath = templateIdOrPathTextNode.Value;

            var itemName = itemIdOrPath.Mid(itemIdOrPath.LastIndexOf('/') + 1);
            var projectUniqueId = itemTextNode.GetAttributeValue("Template.Id", itemIdOrPath);

            var template = context.ParseContext.Factory.Template(context.ParseContext.Project, projectUniqueId, itemTextNode, context.ParseContext.DatabaseName, itemName, itemIdOrPath);

            template.ItemName = itemName;
            template.ItemNameProperty.AddSourceTextNode(templateIdOrPathTextNode);
            template.ItemNameProperty.SourcePropertyFlags = SourcePropertyFlags.IsQualified;

            template.IconProperty.Parse("Template.Icon", itemTextNode);
            template.BaseTemplatesProperty.Parse("Template.BaseTemplates", itemTextNode, Constants.Templates.StandardTemplate);
            template.ShortHelpProperty.Parse("Template.ShortHelp", itemTextNode);
            template.LongHelpProperty.Parse("Template.LongHelp", itemTextNode);

            template.References.AddRange(ParseReferences(context, template, itemTextNode, template.BaseTemplates));

            // section
            var templateSection = context.ParseContext.Factory.TemplateSection(TextNode.Empty);
            template.Sections.Add(templateSection);
            templateSection.SectionNameProperty.SetValue("Fields");
            templateSection.IconProperty.SetValue("Applications/16x16/form_blue.png");

            // fields
            var fieldTreeNodes = context.Snapshot.GetJsonChildTextNode(itemTextNode, "Fields");
            if (fieldTreeNodes != null)
            {
                var nextSortOrder = 0;
                foreach (var child in fieldTreeNodes.ChildNodes)
                {
                    if (child.Name != string.Empty && child.Name != "Field")
                    {
                        continue;
                    }

                    var templateField = context.ParseContext.Factory.TemplateField(template);
                    templateSection.Fields.Add(templateField);

                    templateField.FieldNameProperty.Parse(child);
                    templateField.TypeProperty.Parse("Field.Type", child, "Single-Line Text");
                    templateField.Shared = string.Compare(child.GetAttributeValue("Field.Sharing"), "Shared", StringComparison.OrdinalIgnoreCase) == 0;
                    templateField.Unversioned = string.Compare(child.GetAttributeValue("Field.Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase) == 0;
                    templateField.SourceProperty.Parse("Field.Source", child);
                    templateField.ShortHelpProperty.Parse("Field.ShortHelp", child);
                    templateField.LongHelpProperty.Parse("Field.LongHelp", child);
                    templateField.SortOrderProperty.Parse("Field.SortOrder", child, nextSortOrder);

                    nextSortOrder = templateField.SortOrder + 100;

                    template.References.AddRange(ParseReferences(context, template, child, templateField.Source));
                }
            }

            return context.ParseContext.Project.AddOrMerge(context.ParseContext, template);
        }
    }
}
