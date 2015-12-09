// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.DatabaseProjectItemConditions
{
    public class ItemIdOrPathCondition : StringConditionBase
    {
        public ItemIdOrPathCondition() : base("item-id-or-path")
        {
        }

        protected override string GetValue(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var item = ruleContext.Object as DatabaseProjectItem;
            return item?.ItemIdOrPath ?? string.Empty;
        }
    }
}
