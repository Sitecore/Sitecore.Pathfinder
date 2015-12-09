// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.ItemConditions
{
    public class ChildrenCondition : ConditionBase
    {
        public ChildrenCondition() : base("children")
        {
        }

        public override bool Evaluate(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            int count;
            if (!int.TryParse(GetParameterValue(parameters, "count", ruleContext.Object), out count))
            {
                count = -1;
            }

            var containsName = GetParameterValue(parameters, "contains", ruleContext.Object);
            var any = string.Equals(GetParameterValue(parameters, "any", ruleContext.Object), "true", StringComparison.OrdinalIgnoreCase);

            var item = ruleContext.Object as Item;
            if (item == null)
            {
                return false;
            }

            var children = item.GetChildren();

            if (count >= 0)
            {
                if (children.Count() != count)
                {
                    return false;
                }
            }

            if (containsName != null)
            {
                if (children.All(c => !string.Equals(c.ItemName, containsName, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }
            }

            if (any && !children.Any())
            {
                return false;
            }

            return true;
        }
    }
}
