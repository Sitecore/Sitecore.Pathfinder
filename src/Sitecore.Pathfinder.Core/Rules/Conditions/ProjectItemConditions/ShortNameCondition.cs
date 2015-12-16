// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.ProjectItemConditions
{
    public class ShortNameCondition : StringConditionBase
    {
        public ShortNameCondition() : base("short-name")
        {
        }

        protected override string GetValue(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var item = ruleContext.Object as IProjectItem;
            return item?.ShortName ?? string.Empty;
        }
    }
}
