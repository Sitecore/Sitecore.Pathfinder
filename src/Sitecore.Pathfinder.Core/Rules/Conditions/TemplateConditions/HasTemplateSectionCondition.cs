// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.TemplateConditions
{
    public class HasTemplateSectionCondition : ConditionBase
    {
        public HasTemplateSectionCondition() : base("has-template-section")
        {
        }

        public override bool Evaluate(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var template = ruleContext.Object as Template;
            if (template == null)
            {
                return false;
            }

            object sectionName;
            if (!parameters.TryGetValue("name", out sectionName))
            {
                return false;
            }

            return template.Sections.Any(s => string.Equals(s.SectionName, sectionName as string, StringComparison.OrdinalIgnoreCase));
        }
    }
}
