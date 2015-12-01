// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Rules.Contexts
{
    public class ItemsRuleContext : IProjectItemRuleContext, IItemBasesRuleContext, IItemsRuleContext
    {
        public ItemsRuleContext([NotNull] [ItemNotNull] IEnumerable<Item> items)
        {
            Items = items;
        }

        public ItemsRuleContext([NotNull] Item item)
        {
            Items = new[]
            {
                item
            };
        }

        public IEnumerable<ItemBase> ItemBases => Items;

        public IEnumerable<Item> Items { get; }

        public IEnumerable<IProjectItem> ProjectItems => Items;
    }
}
