using System.Collections.Generic;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.ItemConditions
{
    public class ItemIconCondition : StringConditionBase
    {
        public ItemIconCondition() : base("item-icon")
        {
        }

        protected override string GetValue(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var item = ruleContext.Object as Item;
            return item?.Icon ?? string.Empty;
        }
    }
}