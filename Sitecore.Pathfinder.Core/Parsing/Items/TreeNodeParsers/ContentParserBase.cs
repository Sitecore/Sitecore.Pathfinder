// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

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
            var parentItemPath = textNode.GetAttributeValue("ParentItemPath", context.ParentItemPath);
            var itemIdOrPath = PathHelper.CombineItemPath(parentItemPath, itemNameTextNode.Value);
            var guid = StringHelper.GetGuid(context.ParseContext.Project, textNode.GetAttributeValue("Id", itemIdOrPath));
            var databaseName = textNode.GetAttributeValue("Database", context.DatabaseName);
            var templateIdOrPath = StringHelper.UnescapeXmlNodeName(textNode.Name);

            var item = context.ParseContext.Factory.Item(context.ParseContext.Project, guid, textNode, databaseName, itemNameTextNode.Value, itemIdOrPath, templateIdOrPath);
            item.ItemNameProperty.AddSourceTextNode(itemNameTextNode);
            item.TemplateIdOrPathProperty.AddSourceTextNode(new AttributeNameTextNode(textNode));
            item.IsEmittable = string.Compare(textNode.GetAttributeValue("IsEmittable"), "False", StringComparison.OrdinalIgnoreCase) != 0;
            item.IsExternalReference = string.Compare(textNode.GetAttributeValue("IsExternalReference"), "True", StringComparison.OrdinalIgnoreCase) == 0;

            if (!item.IsExternalReference)
            {
                item.References.AddRange(ParseReferences(context, item, item.TemplateIdOrPathProperty));
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
            var fieldName = StringHelper.UnescapeXmlNodeName(fieldTextNode.Name);

            if (fieldName == "Name" || fieldName == "Id" || fieldName == "ParentItemPath" || fieldName == "IsEmittable" || fieldName == "IsExternalReference" || fieldName == "Database")
            {
                return;
            }

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
                item.References.AddRange(ParseReferences(context, item, field.ValueProperty));
            }
        }
    }
}
