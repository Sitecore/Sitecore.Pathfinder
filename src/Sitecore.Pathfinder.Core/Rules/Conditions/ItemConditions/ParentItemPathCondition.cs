using System.Collections.Generic;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.ItemConditions
{
    public class ParentItemPathCondition : StringConditionBase
    {
        public ParentItemPathCondition() : base("parent-item-path")
        {
        }

        protected override string GetValue(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var item = ruleContext.Object as Item;
            if (item == null)
            {
                return string.Empty;
            }

            var parentItem = item.GetParent();
            return parentItem?.ItemIdOrPath ?? string.Empty;
        }
    }
}