using System.Collections.Generic;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.TemplateConditions
{
    public class TemplateIdOrPathCondition : StringConditionBase
    {
        public TemplateIdOrPathCondition() : base("template-id-or-path")
        {
        }

        protected override string GetValue(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var template = ruleContext.Object as Template;
            return template?.ItemIdOrPath ?? string.Empty;
        }
    }
}