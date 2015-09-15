// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing.Pipelines.ItemParserPipelines;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

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
            var itemIdOrPath = PathHelper.CombineItemPath(context.ParentItemPath, itemNameTextNode.Value);
            var guid = StringHelper.GetGuid(context.ParseContext.Project, textNode.GetAttributeValue("Id", itemIdOrPath));
            var databaseName = textNode.GetAttributeValue("Database", context.DatabaseName);
            var templateIdOrPathTextNode = textNode.GetAttributeTextNode("Template");
            var templateIdOrPath = templateIdOrPathTextNode?.Value ?? string.Empty;

            var item = context.ParseContext.Factory.Item(context.ParseContext.Project, guid, textNode, databaseName, itemNameTextNode.Value, itemIdOrPath, templateIdOrPath);
            item.ItemNameProperty.AddSourceTextNode(itemNameTextNode);
            item.IsEmittable = string.Compare(textNode.GetAttributeValue("IsEmittable"), "False", StringComparison.OrdinalIgnoreCase) != 0;
            item.IsExternalReference = string.Compare(textNode.GetAttributeValue("IsExternalReference"), "True", StringComparison.OrdinalIgnoreCase) == 0;

            if (templateIdOrPathTextNode != null)
            {
                item.TemplateIdOrPathProperty.AddSourceTextNode(templateIdOrPathTextNode);

                if (!item.IsExternalReference)
                {
                    item.References.AddRange(ParseReferences(context, item, item.TemplateIdOrPathProperty));
                }
            }

            context.ParseContext.PipelineService.Resolve<ItemParserPipeline>().Execute(context, item, textNode);

            ParseChildNodes(context, item, textNode);

            var createTemplate = textNode.GetAttributeValue("Template.CreateFromFields");
            if (string.Equals(createTemplate, "true", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(templateIdOrPath))
                {
                    context.ParseContext.Trace.TraceError(Texts.The__Template__attribute_must_be_specified_when__Template_CreateFromFields__equals_true_, textNode);
                }

                ParseTemplate(context, item, templateIdOrPathTextNode);
            }

            context.ParseContext.Project.AddOrMerge(context.ParseContext, item);
        }

        protected abstract void ParseChildNodes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode);

        protected virtual void ParseFieldTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] FieldContext fieldContext, [NotNull] ITextNode fieldTextNode)
        {
            var fieldNameTextNode = fieldTextNode.GetAttributeTextNode("Name");
            if (fieldNameTextNode == null || string.IsNullOrEmpty(fieldNameTextNode.Value))
            {
                context.ParseContext.Trace.TraceError(Texts._Field__element_must_have_a__Name__attribute, fieldTextNode);
                return;
            }

            ParseFieldTreeNode(context, item, fieldContext, fieldTextNode, fieldNameTextNode);
        }

        protected virtual void ParseFieldTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] FieldContext fieldContext, [NotNull] ITextNode fieldTextNode, [NotNull] ITextNode fieldNameTextNode)
        {
            // get field value either from Value attribute or from inner text
            var innerTextNode = fieldTextNode.GetInnerTextNode();
            var attributeTextNode = fieldTextNode.GetAttributeTextNode("Value");

            // todo: fix this
            /*
            if (innerTextNode != null && attributeTextNode != null)
            {
                context.ParseContext.Trace.TraceWarning(Texts.Value_is_specified_in_both__Value__attribute_and_in_element__Using_value_from_attribute, fieldTextNode.Snapshot.SourceFile.FileName, attributeTextNode.Position, fieldName.Value);
            }
            */
            var valueTextNode = attributeTextNode ?? innerTextNode;

            ParseFieldTreeNode(context, item, fieldContext, fieldTextNode, fieldNameTextNode, valueTextNode);
        }

        protected virtual void ParseFieldTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] FieldContext fieldContext, [NotNull] ITextNode fieldTextNode, [NotNull] ITextNode fieldNameTextNode, [CanBeNull] ITextNode valueTextNode)
        {
            var field = context.ParseContext.Factory.Field(item, fieldTextNode);
            field.FieldNameProperty.SetValue(fieldNameTextNode);
            field.LanguageProperty.SetValue(fieldContext.LanguageProperty, SetValueOptions.DisableUpdates);
            field.VersionProperty.SetValue(fieldContext.VersionProperty, SetValueOptions.DisableUpdates);
            field.ValueHintProperty.Parse(fieldTextNode);

            if (valueTextNode != null)
            {
                field.ValueProperty.SetValue(valueTextNode);
            }

            // check if field is already defined
            var duplicate = item.Fields.FirstOrDefault(f => string.Equals(f.FieldName, field.FieldName, StringComparison.OrdinalIgnoreCase) && string.Equals(f.Language, field.Language, StringComparison.OrdinalIgnoreCase) && f.Version == field.Version);
            if (duplicate != null)
            {
                context.ParseContext.Trace.TraceError(Texts.Field_is_already_defined, fieldTextNode, duplicate.FieldName);
            }

            item.Fields.Add(field);

            if (!item.IsExternalReference && !field.ValueHint.Contains("NoReference"))
            {
                item.References.AddRange(ParseReferences(context, item, field.ValueProperty));
            }
        }

        [NotNull]
        protected virtual Template ParseTemplate([NotNull] ItemParseContext context, [NotNull] Item item, [CanBeNull] ITextNode templateIdOrPathTextNode)
        {
            var itemTextNode = item.SourceTextNodes.First();
            var itemIdOrPath = item.TemplateIdOrPath;

            var itemName = itemIdOrPath.Mid(itemIdOrPath.LastIndexOf('/') + 1);
            var guid = StringHelper.GetGuid(context.ParseContext.Project, itemTextNode.GetAttributeValue("Template.Id", itemIdOrPath));
            var template = context.ParseContext.Factory.Template(context.ParseContext.Project, guid, itemTextNode, item.DatabaseName, itemName, itemIdOrPath);

            template.ItemName = itemName;
            template.ItemNameProperty.AddSourceTextNode(templateIdOrPathTextNode);
            template.ItemNameProperty.SourcePropertyFlags = SourcePropertyFlags.IsQualified;

            template.IconProperty.Parse("Template.Icon", itemTextNode);
            template.BaseTemplatesProperty.Parse("Template.BaseTemplates", itemTextNode, Constants.Templates.StandardTemplate);
            template.ShortHelpProperty.Parse("Template.ShortHelp", itemTextNode);
            template.LongHelpProperty.Parse("Template.LongHelp", itemTextNode);
            template.IsEmittable = string.Compare(itemTextNode.GetAttributeValue("IsEmittable"), "False", StringComparison.OrdinalIgnoreCase) != 0;
            template.IsExternalReference = string.Compare(itemTextNode.GetAttributeValue("IsExternalReference"), "True", StringComparison.OrdinalIgnoreCase) == 0;

            if (!template.IsExternalReference)
            {
                template.References.AddRange(ParseReferences(context, template, template.BaseTemplatesProperty));
            }

            if (item.Fields.Any())
            {
                // section
                var templateSection = context.ParseContext.Factory.TemplateSection(TextNode.Empty);
                template.Sections.Add(templateSection);
                templateSection.SectionNameProperty.SetValue("Fields");
                templateSection.IconProperty.SetValue("Applications/16x16/form_blue.png");

                // fields
                var nextSortOrder = 0;
                foreach (var field in item.Fields)
                {
                    var child = field.SourceTextNodes.First();

                    // ignore standard fields
                    if (context.ParseContext.Project.Options.StandardTemplateFields.Contains(field.FieldName, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var templateField = template.Sections.SelectMany(s => s.Fields).FirstOrDefault(f => f.FieldName == field.FieldName);
                    if (templateField == null)
                    {
                        templateField = context.ParseContext.Factory.TemplateField(template, child);
                        templateSection.Fields.Add(templateField);

                        templateField.FieldNameProperty.SetValue(field.FieldNameProperty);
                    }
                    else
                    {
                        // todo: multiple sources?
                        templateField.FieldNameProperty.AddSourceTextNode(field.FieldNameProperty.SourceTextNode);
                    }

                    templateField.TypeProperty.TryParse("Field.Type", child, "Single-Line Text");
                    templateField.Shared |= string.Equals(child.GetAttributeValue("Field.Sharing"), "Shared", StringComparison.OrdinalIgnoreCase);
                    templateField.Unversioned |= string.Equals(child.GetAttributeValue("Field.Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase);
                    templateField.SourceProperty.TryParse("Field.Source", child);
                    templateField.ShortHelpProperty.TryParse("Field.ShortHelp", child);
                    templateField.LongHelpProperty.TryParse("Field.LongHelp", child);
                    templateField.SortOrderProperty.TryParse("Field.SortOrder", child, nextSortOrder);

                    nextSortOrder = templateField.SortOrder + 100;

                    // todo: added multiple times if merged
                    template.References.AddRange(ParseReferences(context, template, templateField.SourceProperty));
                }
            }

            return context.ParseContext.Project.AddOrMerge(context.ParseContext, template);
        }

        protected class FieldContext
        {
            [NotNull]
            public SourceProperty<string> LanguageProperty { get; } = new SourceProperty<string>("Language", string.Empty);

            [NotNull]
            public SourceProperty<int> VersionProperty { get; } = new SourceProperty<int>("Number", 0);
        }
    }
}
