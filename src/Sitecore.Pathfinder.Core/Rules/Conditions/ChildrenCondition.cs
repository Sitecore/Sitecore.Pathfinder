// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions
{
    public class ChildrenCondition : ConditionBase
    {
        public ChildrenCondition() : base("children")
        {
        }

        public override bool Evaluate(IRuleContext ruleContext, IDictionary<string, string> parameters)
        {
            if (!ruleContext.Objects.Any())
            {
                return false;
            }

            int count;
            if (!int.TryParse(parameters.GetString("count"), out count))
            {
                count = -1;
            }

            var containsName = parameters.GetString("contains");
            var any = string.Equals(parameters.GetString("any"), "true", StringComparison.OrdinalIgnoreCase);

            foreach (var obj in ruleContext.Objects)
            {
                var item = obj as Item;
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

                if (!string.IsNullOrEmpty(containsName))
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
            }

            return true;
        }
    }
}
