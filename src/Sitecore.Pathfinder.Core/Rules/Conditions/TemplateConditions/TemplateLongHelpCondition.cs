using System.Collections.Generic;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.TemplateConditions
{
    public class TemplateLongHelpCondition : StringConditionBase
    {
        public TemplateLongHelpCondition() : base("template-long-help")
        {
        }

        protected override string GetValue(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var template = ruleContext.Object as Template;
            return template?.LongHelp ?? string.Empty;
        }
    }
}