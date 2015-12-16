// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.TemplateConditions
{
    public class TemplateSectionCountCondition : IntConditionBase
    {
        public TemplateSectionCountCondition() : base("template-section-count")
        {
        }

        protected override int GetValue(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var template = ruleContext.Object as Template;
            return template?.Sections.Count ?? 0;
        }
    }
}
