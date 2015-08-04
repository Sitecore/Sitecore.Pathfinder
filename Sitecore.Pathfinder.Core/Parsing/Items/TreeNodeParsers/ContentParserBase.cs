// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
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
            var itemName = GetItemName(context.ParseContext, textNode);
            var parentItemPath = textNode.GetAttributeValue("Parent-Item-Path", context.ParentItemPath);
            var itemIdOrPath = parentItemPath + "/" + itemName;
            var projectUniqueId = textNode.GetAttributeValue("Id", itemIdOrPath);

            var item = context.ParseContext.Factory.Item(context.ParseContext.Project, projectUniqueId, textNode, context.ParseContext.DatabaseName, itemName, itemIdOrPath, textNode.Name);
            item.ItemName.Merge(itemName);
            item.TemplateIdOrPath.AddSource(new AttributeNameTextNode(textNode));

            item.References.AddRange(ParseReferences(context, item, textNode, item.TemplateIdOrPath));

            ParseAttributes(context, item, textNode);

            context.ParseContext.Project.AddOrMerge(context.ParseContext, item);
        }

        protected abstract void ParseAttributes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode);

        protected virtual void ParseFieldTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode fieldTextNode)
        {
            var fieldName = fieldTextNode.Name;

            if (fieldName == "Name" || fieldName == "Id" || fieldName == "Parent-Item-Path")
            {
                return;
            }
            
            // check if field is already defined
            var field = item.Fields.FirstOrDefault(f => string.Compare(f.FieldName.Value, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
            if (field != null)
            {
                context.ParseContext.Trace.TraceError(Texts.Field_is_already_defined, fieldTextNode.Snapshot.SourceFile.FileName, fieldTextNode.Position, fieldName);
            }

            // todo: support language and version
            field = context.ParseContext.Factory.Field(item);
            field.FieldName.AddSource(new AttributeNameTextNode(fieldTextNode));
            field.Value.AddSource(fieldTextNode);

            item.Fields.Add(field);

            item.References.AddRange(ParseReferences(context, item, fieldTextNode, field.Value));
        }
    }
}
