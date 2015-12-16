// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.DatabaseProjectItemConditions
{
    public class DatabaseNameCondition : StringConditionBase
    {
        public DatabaseNameCondition() : base("database-name")
        {
        }

        protected override string GetValue(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var item = ruleContext.Object as DatabaseProjectItem;
            return item?.DatabaseName ?? string.Empty;
        }
    }
}
