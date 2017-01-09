// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing.Pipelines.ItemParserPipelines;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Parsing.Items
{
    public abstract class ItemTextNodeParserBase : TextNodeParserBase
    {
        protected ItemTextNodeParserBase([NotNull] ISchemaService schemaService, double priority) : base(priority)
        {
            SchemaService = schemaService;
        }

        [NotNull]
        protected ISchemaService SchemaService { get; }

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
               context.ParseContext.Trace.TraceError(Msg.P1000, Texts.Item_name_in__ItemPath__and__Name__does_not_match__Using__Name_);
            }

            var guid = StringHelper.GetGuid(context.ParseContext.Project, textNode.GetAttributeValue("Id", itemIdOrPath));
            var databaseName = textNode.GetAttributeValue("Database", context.DatabaseName);
            var templateIdOrPathTextNode = textNode.GetAttribute("Template");
            var templateIdOrPath = templateIdOrPathTextNode?.Value ?? string.Empty;

            var item = context.ParseContext.Factory.Item(context.ParseContext.Project, guid, databaseName, itemNameTextNode.Value, itemIdOrPath, templateIdOrPath).With(textNode);
            item.ItemNameProperty.AddSourceTextNode(itemNameTextNode);
            item.IconProperty.Parse(textNode);
            // todo: yuck
            item.IsEmittable = !string.Equals(textNode.GetAttributeValue(Constants.Fields.IsEmittable), "False", StringComparison.OrdinalIgnoreCase);
            item.IsImport = string.Equals(textNode.GetAttributeValue(Constants.Fields.IsImport, context.IsImport.ToString()), "True", StringComparison.OrdinalIgnoreCase);
            item.SortorderProperty.Parse(textNode, context.Sortorder);

            if (templateIdOrPathTextNode != null)
            {
                item.TemplateIdOrPathProperty.AddSourceTextNode(templateIdOrPathTextNode);

                if (!item.IsImport)
                {
                    item.References.AddRange(context.ParseContext.ReferenceParser.ParseReferences(item, item.TemplateIdOrPathProperty));
                }
            }

            ParseChildNodes(context, item, textNode);

            context.ParseContext.Pipelines.Resolve<ItemParserPipeline>().Execute(context, item, textNode);

            context.ParseContext.Project.AddOrMerge(item);
        }

        protected virtual void ParseChildNodes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode)
        {
            foreach (var childNode in textNode.ChildNodes)
            {
                switch (childNode.Key)
                {
                    case "Fields":
                        ParseFieldsTextNode(context, item, childNode);
                        break;

                    case "Children":
                        ParseChildrenTextNodes(context, item, childNode);
                        break;

                    default:
                        var newContext = context.ParseContext.Factory.ItemParseContext(context.ParseContext, context.Parser, item.DatabaseName, PathHelper.CombineItemPath(context.ParentItemPath, item.ItemName), item.IsImport);
                        context.Parser.ParseTextNode(newContext, childNode);
                        break;
                }
            }
        }

        protected virtual void ParseChildrenTextNodes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode)
        {
            var sortorder = 100;
            foreach (var childNode in textNode.ChildNodes)
            {
                var newContext = context.ParseContext.Factory.ItemParseContext(context.ParseContext, context.Parser, item.DatabaseName, PathHelper.CombineItemPath(context.ParentItemPath, item.ItemName), item.IsImport).With(sortorder);
                context.Parser.ParseTextNode(newContext, childNode);
                sortorder += 100;
            }
        }

        protected virtual void ParseFieldsTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode fieldsTextNode)
        {
            var languageVersionContext = new LanguageVersionContext();

            foreach (var childNode in fieldsTextNode.ChildNodes)
            {
                switch (childNode.Key)
                {
                    case "Field":
                        ParseFieldTextNode(context, item, languageVersionContext, childNode);
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
                        ParseUnknownTextNode(context, item, languageVersionContext, childNode);
                        break;
                }
            }
        }

        protected virtual void ParseFieldTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] LanguageVersionContext languageVersionContext, [NotNull] ITextNode textNode)
        {
            var fieldNameTextNode = textNode.GetAttribute("Name");
            if (fieldNameTextNode == null || string.IsNullOrEmpty(fieldNameTextNode.Value))
            {
                context.ParseContext.Trace.TraceError(Msg.P1010, Texts._Field__element_must_have_a__Name__attribute, textNode);
                return;
            }

            ParseFieldTextNode(context, item, languageVersionContext, textNode, fieldNameTextNode);
        }

        protected virtual void ParseFieldTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] LanguageVersionContext languageVersionContext, [NotNull] ITextNode textNode, [NotNull] ITextNode fieldNameTextNode)
        {
            // get field value either from Value attribute or from inner text
            var innerTextNode = textNode.GetInnerTextNode();
            var attributeTextNode = textNode.GetAttribute("Value");

            // todo: fix this
            /*
            if (innerTextNode != null && attributeTextNode != null)
            {
                context.ParseContext.Trace.TraceWarning(Texts.Value_is_specified_in_both__Value__attribute_and_in_element__Using_value_from_attribute, fieldTextNode.Snapshot.SourceFile.FileName, attributeTextNode.Position, fieldName.Value);
            }
            */
            var valueTextNode = attributeTextNode ?? innerTextNode;

            ParseFieldTextNode(context, item, languageVersionContext, textNode, fieldNameTextNode, valueTextNode);
        }

        protected virtual void ParseFieldTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] LanguageVersionContext languageVersionContext, [NotNull] ITextNode fieldTextNode, [NotNull] ITextNode fieldNameTextNode, [CanBeNull] ITextNode valueTextNode)
        {
            SchemaService.ValidateTextNodeSchema(fieldTextNode);

            var field = context.ParseContext.Factory.Field(item).With(fieldTextNode);
            field.FieldNameProperty.SetValue(fieldNameTextNode);
            field.LanguageProperty.SetValue(languageVersionContext.LanguageProperty);
            field.VersionProperty.SetValue(languageVersionContext.VersionProperty);
            field.ValueHintProperty.Parse(fieldTextNode);

            if (valueTextNode != null)
            {
                field.ValueProperty.SetValue(valueTextNode);
            }

            // check if field is already defined
            var duplicate = item.Fields.FirstOrDefault(f => string.Equals(f.FieldName, field.FieldName, StringComparison.OrdinalIgnoreCase) && f.Language == field.Language && f.Version == field.Version);
            if (duplicate == null)
            {
                item.Fields.Add(field);
            }
            else
            {
                context.ParseContext.Trace.TraceError(Msg.P1011, Texts.Field_is_already_defined, fieldTextNode, duplicate.FieldName);
            }

            if (!item.IsImport && !field.ValueHint.Contains("NoReference"))
            {
                var references = context.ParseContext.ReferenceParser.ParseReferences(field);
                item.References.AddRange(references);
            }
        }

        protected abstract void ParseLayoutTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode);

        protected abstract void ParseUnknownTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] LanguageVersionContext languageVersionContext, [NotNull] ITextNode textNode);

        protected abstract void ParseUnversionedTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode);

        protected abstract void ParseVersionedTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode);
    }
}
