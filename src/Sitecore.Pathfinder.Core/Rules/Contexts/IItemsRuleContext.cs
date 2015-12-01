// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Rules.Contexts
{
    public interface IItemsRuleContext : IRuleContext
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<Item> Items { get; }
    }
}
