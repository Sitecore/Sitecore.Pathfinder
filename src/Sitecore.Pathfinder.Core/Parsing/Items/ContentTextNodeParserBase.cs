// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing.References;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Parsing.Items
{
    public abstract class ContentTextNodeParserBase : TextNodeParserBase
    {
        [NotNull]
        protected ITraceService Trace { get; }

        [NotNull]
        protected IReferenceParserService ReferenceParser { get; }

        [NotNull]
        protected ISchemaService SchemaService { get; }

        protected ContentTextNodeParserBase([NotNull] ITraceService trace, [NotNull] IReferenceParserService referenceParser, [NotNull] ISchemaService schemaService, double priority) : base(priority)
        {
            Trace = trace;
            ReferenceParser = referenceParser;
            SchemaService = schemaService;
        }

        public override void Parse(ItemParseContext context, ITextNode textNode)
        {
            SchemaService.ValidateTextNodeSchema(textNode, "Item");

            var itemNameTextNode = GetItemNameTextNode(context.ParseContext, textNode);
            var parentItemPath = textNode.GetAttributeValue("ParentItemPath", context.ParentItemPath);
            var itemIdOrPath = textNode.GetAttributeValue("ItemPath");
            if (string.IsNullOrEmpty(itemIdOrPath))
            {
                itemIdOrPath = PathHelper.CombineItemPath(parentItemPath, itemNameTextNode.Value);
            }
            else if (itemNameTextNode.Value != Path.GetFileName(itemIdOrPath))
            {
                Trace.TraceError(Msg.P1027, "Item name in 'ItemPath' and 'Name' does not match. Using 'Name'");
            }

            var guid = StringHelper.GetGuid(context.ParseContext.Project, textNode.GetAttributeValue("Id", itemIdOrPath));
            var databaseName = textNode.GetAttributeValue("Database", context.Database.DatabaseName);
            var database = context.ParseContext.Project.GetDatabase(databaseName);
            var templateIdOrPath = textNode.GetAttributeValue("TemplateName", textNode.Key.UnescapeXmlElementName());

            var item = context.ParseContext.Factory.Item(database, guid, itemNameTextNode.Value, itemIdOrPath, templateIdOrPath).With(textNode);
            item.ItemNameProperty.AddSourceTextNode(itemNameTextNode);
            item.TemplateIdOrPathProperty.AddSourceTextNode(new AttributeNameTextNode(textNode));
            item.SortorderProperty.Parse(textNode, context.Sortorder);
            item.IsEmittable = textNode.GetAttributeBool(Constants.Fields.IsEmittable, true);
            item.IsImport = textNode.GetAttributeBool(Constants.Fields.IsImport, context.IsImport);

            if (!item.IsImport)
            {
                item.References.AddRange(ReferenceParser.ParseReferences(item, item.TemplateIdOrPathProperty));
            }

            // parse shared fields
            var fieldContext = new LanguageVersionContext();
            ParseAttributes(context, item, fieldContext, textNode);

            // parse Fields, Layout and Items child text nodes
            ParseChildNodes(context, item, textNode);

            context.ParseContext.Project.AddOrMerge(item);
        }

        protected virtual void ParseAttributes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] LanguageVersionContext languageVersionContext, [NotNull] ITextNode textNode)
        {
            foreach (var childNode in textNode.Attributes)
            {
                ParseFieldTextNode(context, item, languageVersionContext, childNode);
            }
        }

        protected virtual void ParseChildNodes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode)
        {
            var sortorder = 100;
            foreach (var childNode in textNode.ChildNodes)
            {
                switch (childNode.Key)
                {
                    case "Fields":
                    case "fields":
                        ParseFieldsTextNode(context, item, childNode);
                        break;

                    case "Layout":
                    case "layout":
                        ParseLayoutTextNode(context, item, childNode);
                        break;

                    case "Items":
                    case "items":
                        ParseItemsTextNode(context, item, childNode, sortorder);
                        sortorder += 100;
                        break;

                    default:
                        Trace.TraceError(Msg.P1026, "Unexpected text node", childNode, childNode.Key);
                        break;
                }
            }
        }

        protected abstract void ParseFieldsTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode fieldsTextNode);

        protected virtual void ParseFieldTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] LanguageVersionContext languageVersionContext, [NotNull] ITextNode fieldTextNode)
        {
            SchemaService.ValidateTextNodeSchema(fieldTextNode, "Field");

            var fieldName = fieldTextNode.Key.UnescapeXmlElementName();
            if (fieldName == "Name" || fieldName == "Id" || fieldName == "Database" || fieldName == "TemplateName" || fieldName == "ItemPath" || fieldName == "ParentItemPath" || fieldName == Constants.Fields.IsEmittable || fieldName == Constants.Fields.IsImport)
            {
                return;
            }

            var field = context.ParseContext.Factory.Field(item).With(fieldTextNode);
            field.FieldNameProperty.SetValue(new AttributeNameTextNode(fieldTextNode));
            field.LanguageProperty.SetValue(languageVersionContext.LanguageProperty);
            field.VersionProperty.SetValue(languageVersionContext.VersionProperty);
            field.ValueProperty.SetValue(fieldTextNode);

            // check if field is already defined
            var duplicate = item.Fields.FirstOrDefault(f => string.Equals(f.FieldName, field.FieldName, StringComparison.OrdinalIgnoreCase) && f.Language == field.Language && f.Version == field.Version);
            if (duplicate == null)
            {
                item.Fields.Add(field);
            }
            else
            {
                Trace.TraceError(Msg.P1008, Texts.Field_is_already_defined, fieldTextNode, duplicate.FieldName);
            }

            if (!item.IsImport)
            {
                item.References.AddRange(ReferenceParser.ParseReferences(field));
            }
        }

        protected virtual void ParseItemsTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode itemsTextNode, int sortorder)
        {
            foreach (var childNode in itemsTextNode.ChildNodes)
            {
                var newContext = context.ParseContext.Factory.ItemParseContext(context.ParseContext, context.Parser, item.Database, item.ItemIdOrPath, item.IsImport).With(sortorder);
                context.Parser.ParseTextNode(newContext, childNode);
            }
        }

        protected abstract void ParseLayoutTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode layoutTextNode);
    }
}
