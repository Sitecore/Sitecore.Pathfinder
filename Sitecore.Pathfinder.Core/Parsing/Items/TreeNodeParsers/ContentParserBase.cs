// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
    public abstract class ContentParserBase : TextNodeParserBase
    {
        protected ContentParserBase(double priority) : base(priority)
        {
        }

        public override void Parse(ItemParseContext context, ITextNode textNode)
        {
            var itemNameTextNode = textNode.GetTextNodeAttribute("Item-Name");
            var itemName = itemNameTextNode?.Value ?? context.ParseContext.ItemName;
            var parentItemPath = textNode.GetAttributeValue("Parent-Item-Path", context.ParentItemPath);
            var itemIdOrPath = parentItemPath + "/" + itemName;
            var projectUniqueId = textNode.GetAttributeValue("Id", itemIdOrPath);

            var item = context.ParseContext.Factory.Item(context.ParseContext.Project, projectUniqueId, textNode, context.ParseContext.DatabaseName, itemName, itemIdOrPath, textNode.Name);
            item.ItemName.Source = itemNameTextNode ?? new FileNameTextNode(itemName, textNode.Snapshot);
            item.TemplateIdOrPath.Source = textNode;

            item.References.AddRange(ParseReferences(context, item, textNode, item.TemplateIdOrPath.Value));

            ParseAttributes(context, item, textNode);

            context.ParseContext.Project.AddOrMerge(item);
        }

        protected abstract void ParseAttributes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode);

        protected virtual void ParseFieldTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode fieldTextNode)
        {
            var fieldName = fieldTextNode.Name;

            if (fieldName == "Item-Name")
            {
                return;
            }

            if (fieldName == "Parent-Item-Path")
            {
                return;
            }

            var field = item.Fields.FirstOrDefault(f => string.Compare(f.FieldName.Value, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
            if (field != null)
            {
                context.ParseContext.Trace.TraceError(Texts.Field_is_already_defined, fieldTextNode.Snapshot.SourceFile.FileName, fieldTextNode.Position, fieldName);
            }

            // todo: support for language, version and value.hint
            field = context.ParseContext.Factory.Field(item, fieldName, string.Empty, 0, fieldTextNode.Value, string.Empty);
            field.Value.Source = fieldTextNode;
            item.Fields.Add(field);

            item.References.AddRange(ParseReferences(context, item, fieldTextNode, field.Value.Value));
        }
    }
}
