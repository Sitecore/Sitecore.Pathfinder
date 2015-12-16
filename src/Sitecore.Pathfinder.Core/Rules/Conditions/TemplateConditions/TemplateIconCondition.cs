// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.TemplateConditions
{
    public class TemplateIconCondition : StringConditionBase
    {
        public TemplateIconCondition() : base("template-icon")
        {
        }

        protected override string GetValue(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var template = ruleContext.Object as Template;
            return template?.Icon ?? string.Empty;
        }
    }
}
