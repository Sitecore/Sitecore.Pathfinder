// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.ProjectItemConditions
{
    public class ReferenceCountCondition : IntConditionBase
    {
        public ReferenceCountCondition() : base("reference-count")
        {
        }

        protected override int GetValue(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var item = ruleContext.Object as IProjectItem;
            return item?.References.Count(r => r.IsValid) ?? 0;
        }
    }
}
