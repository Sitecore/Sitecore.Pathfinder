// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.TemplateConditions
{
    public class TemplateSectionFieldCountCondition : IntConditionBase
    {
        public TemplateSectionFieldCountCondition() : base("template-section-field-count")
        {
        }

        protected override int GetValue(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var template = ruleContext.Object as Template;
            if (template == null)
            {
                return 0;
            }

            object sectionName;
            if (!parameters.TryGetValue("name", out sectionName))
            {
                return 0;
            }

            var section = template.Sections.FirstOrDefault(s => string.Equals(s.SectionName, sectionName as string, StringComparison.OrdinalIgnoreCase));
            if (section == null)
            {
                return 0;
            }

            return section.Fields.Count;
        }
    }
}
