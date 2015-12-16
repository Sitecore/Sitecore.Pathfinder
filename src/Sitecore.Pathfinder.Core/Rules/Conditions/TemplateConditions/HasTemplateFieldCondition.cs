// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.TemplateConditions
{
    public class HasTemplateFieldCondition : ConditionBase
    {
        public HasTemplateFieldCondition() : base("has-template-field")
        {
        }

        public override bool Evaluate(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var template = ruleContext.Object as Template;
            if (template == null)
            {
                return false;
            }

            object fieldName;
            if (!parameters.TryGetValue("name", out fieldName))
            {
                return false;
            }

            return template.Sections.SelectMany(s => s.Fields).Any(f => string.Equals(f.FieldName, fieldName as string, StringComparison.OrdinalIgnoreCase));
        }
    }
}
