// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.ItemConditions
{
    public class TemplateNameCondition : StringConditionBase
    {
        public TemplateNameCondition() : base("template-name")
        {
        }

        protected override string GetValue(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var item = ruleContext.Object as Item;
            return item?.TemplateName ?? string.Empty;
        }
    }
}
