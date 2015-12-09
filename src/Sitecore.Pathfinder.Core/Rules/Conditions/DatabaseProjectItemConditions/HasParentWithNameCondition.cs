// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.DatabaseProjectItemConditions
{
    public class HasParentWithNameCondition : ConditionBase
    {
        public HasParentWithNameCondition() : base("has-parent-with-name")
        {
        }

        public override bool Evaluate(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var parentName = GetParameterValue(parameters, "value", ruleContext.Object);
            if (parentName == null)
            {
                throw new ConfigurationException(Texts.Parent_name_expected);
            }

            var item = ruleContext.Object as DatabaseProjectItem;
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

            return true;
        }
    }
}
