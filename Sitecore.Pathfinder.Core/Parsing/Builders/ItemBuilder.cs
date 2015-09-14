// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Parsing.Builders
{
    public class ItemBuilder
    {
        [NotNull]
        public string DatabaseName { get; set; } = string.Empty;

        [NotNull]
        [ItemNotNull]
        public IList<FieldBuilder> Fields { get; } = new List<FieldBuilder>();

        [NotNull]
        public string Guid { get; set; } = string.Empty;

        [NotNull]
        public string Icon { get; set; } = string.Empty;

        [NotNull]
        public ITextNode IconTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public string ItemIdOrPath { get; set; } = string.Empty;

        [NotNull]
        public string ItemName { get; set; } = string.Empty;

        [NotNull]
        public ITextNode ItemNameTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public string TemplateIdOrPath { get; set; } = string.Empty;

        [NotNull]
        public ITextNode TemplateIdOrPathTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public Item Build([NotNull] IParseContext context, [NotNull] ITextNode rootTextNode)
        {
            var guid = StringHelper.GetGuid(context.Project, Guid);

            var item = context.Factory.Item(context.Project, guid, rootTextNode, DatabaseName, ItemName, ItemIdOrPath, TemplateIdOrPath);

            if (ItemNameTextNode != TextNode.Empty)
            {
                item.ItemNameProperty.AddSourceTextNode(ItemNameTextNode);
            }

            if (TemplateIdOrPathTextNode != TextNode.Empty)
            {
                item.TemplateIdOrPathProperty.AddSourceTextNode(TemplateIdOrPathTextNode);
            }

            item.Icon = Icon;
            if (IconTextNode != TextNode.Empty)
            {
                item.IconProperty.AddSourceTextNode(IconTextNode);
            }

            foreach (var fieldBuilder in Fields)
            {
                var field = fieldBuilder.Build(context, item);
                item.Fields.Add(field);
            }

            return item;
        }
    }
}
