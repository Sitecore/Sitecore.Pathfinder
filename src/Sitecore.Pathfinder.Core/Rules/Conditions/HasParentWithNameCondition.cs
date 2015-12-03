// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions
{
    public class HasParentWithNameCondition : ConditionBase
    {
        public HasParentWithNameCondition() : base("has-parent-with-name")
        {
        }

        public override bool Evaluate(IRuleContext ruleContext, IDictionary<string, string> parameters)
        {
            if (!ruleContext.Objects.Any())
            {
                return false;
            }

            var parentName = parameters.GetString("value");

            foreach (var obj in ruleContext.Objects)
            {
                var item = obj as DatabaseProjectItem;
                if (item == null)
                {
                    return false;
                }

                // guess: if ItemIdOrPath is ID, this fails
                var parents = item.ItemIdOrPath.Split(Constants.Slash, StringSplitOptions.RemoveEmptyEntries);

                if (parents.All(name => !string.Equals(name, parentName, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
