// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Compiling.Builders
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ItemBuilder
    {
        [ImportingConstructor]
        public ItemBuilder([NotNull] IFactoryService factory)
        {
            Factory = factory;
        }

        [NotNull]
        public string DatabaseName { get; set; } = string.Empty;

        [NotNull]
        [ItemNotNull]
        public IList<FieldBuilder> Fields { get; } = new List<FieldBuilder>();

        [NotNull]
        public string Guid { get; set; } = string.Empty;

        [NotNull]
        public string HtmlFile { get; set; } = string.Empty;

        [NotNull]
        public ITextNode HtmlFileTextNode { get; set; } = TextNode.Empty;

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

        public bool TemplateCreateFromFields { get; set; }

        [NotNull]
        public string TemplateIcon { get; set; } = string.Empty;

        [NotNull]
        public ITextNode TemplateIconTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public string TemplateIdOrPath { get; set; } = string.Empty;

        [NotNull]
        public ITextNode TemplateIdOrPathTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public string TemplateLongHelp { get; set; } = string.Empty;

        [NotNull]
        public ITextNode TemplateLongHelpTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public string TemplateShortHelp { get; set; } = string.Empty;

        [NotNull]
        public ITextNode TemplateShortHelpTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull]
        public Item Build([NotNull] IProject project, [NotNull] ITextNode rootTextNode)
        {
            var guid = StringHelper.GetGuid(project, Guid);

            var item = Factory.Item(project, guid, rootTextNode, DatabaseName, ItemName, ItemIdOrPath, TemplateIdOrPath);

            if (ItemNameTextNode != TextNode.Empty)
            {
                item.ItemNameProperty.AddSourceTextNode(ItemNameTextNode);
            }

            if (TemplateIdOrPathTextNode != TextNode.Empty)
            {
                item.TemplateIdOrPathProperty.AddSourceTextNode(TemplateIdOrPathTextNode);
            }

            item.IconProperty.SetValue(IconTextNode);

            foreach (var fieldBuilder in Fields)
            {
                var field = fieldBuilder.Build(item);
                item.Fields.Add(field);
            }

            return item;
        }
    }
}
