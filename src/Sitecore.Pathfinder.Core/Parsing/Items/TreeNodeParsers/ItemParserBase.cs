// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing.Pipelines.ItemParserPipelines;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
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
                    item.References.AddRange(context.ParseContext.ReferenceParser.ParseReferences(item, item.TemplateIdOrPathProperty));
                }
            }

            context.ParseContext.PipelineService.Resolve<ItemParserPipeline>().Execute(context, item, textNode);

            ParseChildNodes(context, item, textNode);

            context.ParseContext.Project.AddOrMerge(item);
        }

        protected virtual void ParseChildNodes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode)
        {
            foreach (var childNode in textNode.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "Fields":
                        ParseFieldsTextNode(context, item, childNode);
                        break;

                    default:
                        var newContext = context.ParseContext.Factory.ItemParseContext(context.ParseContext, context.Parser, item.DatabaseName, PathHelper.CombineItemPath(context.ParentItemPath, item.ItemName));
                        context.Parser.ParseTextNode(newContext, childNode);
                        break;
                }
            }
        }

        protected virtual void ParseFieldsTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode)
        {
            var fieldContext = new FieldContext();

            foreach (var childNode in textNode.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "Field":
                        ParseFieldTextNode(context, item, fieldContext, childNode);
                        break;

                    case "Unversioned":
                        ParseUnversionedTextNode(context, item, childNode);
                        break;

                    case "Versioned":
                        ParseVersionedTextNode(context, item, childNode);
                        break;

                    case "Layout":
                        ParseLayoutTextNode(context, item, childNode);
                        break;

                    default:
                        ParseUnknownTextNode(context, item, fieldContext, childNode);
                        break;
                }
            }
        }

        protected virtual void ParseFieldTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] FieldContext fieldContext, [NotNull] ITextNode textNode)
        {
            var fieldNameTextNode = textNode.GetAttributeTextNode("Name");
            if (fieldNameTextNode == null || string.IsNullOrEmpty(fieldNameTextNode.Value))
            {
                context.ParseContext.Trace.TraceError(Texts._Field__element_must_have_a__Name__attribute, textNode);
                return;
            }

            ParseFieldTextNode(context, item, fieldContext, textNode, fieldNameTextNode);
        }

        protected virtual void ParseFieldTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] FieldContext fieldContext, [NotNull] ITextNode textNode, [NotNull] ITextNode fieldNameTextNode)
        {
            // get field value either from Value attribute or from inner text
            var innerTextNode = textNode.GetInnerTextNode();
            var attributeTextNode = textNode.GetAttributeTextNode("Value");

            // todo: fix this
            /*
            if (innerTextNode != null && attributeTextNode != null)
            {
                context.ParseContext.Trace.TraceWarning(Texts.Value_is_specified_in_both__Value__attribute_and_in_element__Using_value_from_attribute, fieldTextNode.Snapshot.SourceFile.FileName, attributeTextNode.Position, fieldName.Value);
            }
            */
            var valueTextNode = attributeTextNode ?? innerTextNode;

            ParseFieldTextNode(context, item, fieldContext, textNode, fieldNameTextNode, valueTextNode);
        }

        protected virtual void ParseFieldTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] FieldContext fieldContext, [NotNull] ITextNode textNode, [NotNull] ITextNode fieldNameTextNode, [CanBeNull] ITextNode valueTextNode)
        {
            var field = context.ParseContext.Factory.Field(item, textNode);
            field.FieldNameProperty.SetValue(fieldNameTextNode);
            field.LanguageProperty.SetValue(fieldContext.LanguageProperty, SetValueOptions.DisableUpdates);
            field.VersionProperty.SetValue(fieldContext.VersionProperty, SetValueOptions.DisableUpdates);
            field.ValueHintProperty.Parse(textNode);

            if (valueTextNode != null)
            {
                field.ValueProperty.SetValue(valueTextNode);
            }

            // check if field is already defined
            var duplicate = item.Fields.FirstOrDefault(f => string.Equals(f.FieldName, field.FieldName, StringComparison.OrdinalIgnoreCase) && string.Equals(f.Language, field.Language, StringComparison.OrdinalIgnoreCase) && f.Version == field.Version);
            if (duplicate == null)
            {
                item.Fields.Add(field);
            }
            else
            {
                context.ParseContext.Trace.TraceError(Texts.Field_is_already_defined, textNode, duplicate.FieldName);
            }

            if (!item.IsExternalReference && !field.ValueHint.Contains("NoReference"))
            {
                item.References.AddRange(context.ParseContext.ReferenceParser.ParseReferences(item, field.ValueProperty));
            }
        }

        protected abstract void ParseLayoutTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode);

        protected virtual void ParseUnknownTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] FieldContext fieldContext, [NotNull] ITextNode textNode)
        {
        }

        protected abstract void ParseUnversionedTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode);

        protected abstract void ParseVersionedTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode);
    }
}
