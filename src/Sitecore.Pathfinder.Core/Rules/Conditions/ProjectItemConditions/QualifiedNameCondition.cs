// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.ProjectItemConditions
{
    public class QualifiedNameCondition : StringConditionBase
    {
        public QualifiedNameCondition() : base("qualified-name")
        {
        }

        protected override string GetValue(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var item = ruleContext.Object as IProjectItem;
            return item?.QualifiedName ?? string.Empty;
        }
    }
}
