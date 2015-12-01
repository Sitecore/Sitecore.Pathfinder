using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Rules.Contexts
{
    public interface IItemBasesRuleContext : IRuleContext
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<ItemBase> ItemBases { get; }
    }
}