// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
    public abstract class ContentParserBase : TextNodeParserBase
    {
        protected ContentParserBase(double priority) : base(priority)
        {
        }

        public override void Parse(ItemParseContext context, ITextNode textNode)
        {
            var itemNameTextNode = GetItemNameTextNode(context.ParseContext, textNode);
            var parentItemPath = textNode.GetAttributeValue("Parent-Item-Path", context.ParentItemPath);
            var itemIdOrPath = PathHelper.CombineItemPath(parentItemPath, itemNameTextNode.Value);
            var projectUniqueId = textNode.GetAttributeValue("Id", itemIdOrPath);
            var databaseName = textNode.GetAttributeValue("Database", context.DatabaseName);

            var item = context.ParseContext.Factory.Item(context.ParseContext.Project, projectUniqueId, textNode, databaseName, itemNameTextNode.Value, itemIdOrPath, textNode.Name);
            item.ItemNameProperty.AddSourceTextNode(itemNameTextNode);
            item.TemplateIdOrPathProperty.AddSourceTextNode(new AttributeNameTextNode(textNode));
            item.IsEmittable = string.Compare(textNode.GetAttributeValue("IsEmittable"), "False", StringComparison.OrdinalIgnoreCase) != 0;
            item.IsExternalReference = string.Compare(textNode.GetAttributeValue("IsExternalReference"), "True", StringComparison.OrdinalIgnoreCase) == 0;

            if (!item.IsExternalReference)
            {
                item.References.AddRange(ParseReferences(context, item, textNode, item.TemplateIdOrPath));
            }

            ParseAttributes(context, item, textNode);

            ParseChildNodes(context, item, textNode);

            context.ParseContext.Project.AddOrMerge(context.ParseContext, item);
        }

        protected virtual void ParseAttributes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode)
        {
            foreach (var childTreeNode in textNode.Attributes)
            {
                ParseFieldTreeNode(context, item, childTreeNode);
            }
        }

        protected virtual void ParseChildNodes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode)
        {
            foreach (var childTreeNode in textNode.ChildNodes)
            {
                var newContext = context.ParseContext.Factory.ItemParseContext(context.ParseContext, context.Parser, item.DatabaseName, item.ItemIdOrPath);
                context.Parser.ParseTextNode(newContext, childTreeNode);
            }
        }

        protected virtual void ParseFieldTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode fieldTextNode)
        {
            var fieldName = fieldTextNode.Name;

            if (fieldName == "Name" || fieldName == "Id" || fieldName == "Parent-Item-Path" || fieldName == "IsEmittable" || fieldName == "IsExternalReference" || fieldName == "Database")
            {
                return;
            }

            // support for spaces in field names - use "--"
            fieldName = fieldName.Replace("--", " ");

            // check if field is already defined
            var field = item.Fields.FirstOrDefault(f => string.Compare(f.FieldName, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
            if (field != null)
            {
                context.ParseContext.Trace.TraceError(Texts.Field_is_already_defined, fieldTextNode.Snapshot.SourceFile.FileName, fieldTextNode.Position, fieldName);
            }

            // todo: support language and version
            field = context.ParseContext.Factory.Field(item, fieldTextNode);
            field.FieldNameProperty.SetValue(new AttributeNameTextNode(fieldTextNode));
            field.ValueProperty.SetValue(fieldTextNode);

            item.Fields.Add(field);

            if (!item.IsExternalReference)
            {
                item.References.AddRange(ParseReferences(context, item, fieldTextNode, field.Value));
            }
        }
    }
}
