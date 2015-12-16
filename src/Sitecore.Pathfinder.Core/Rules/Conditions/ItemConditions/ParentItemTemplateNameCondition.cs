// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.ItemConditions
{
    public class ParentItemTemplateNameCondition : StringConditionBase
    {
        public ParentItemTemplateNameCondition() : base("parent-item-template-name")
        {
        }

        protected override string GetValue(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var item = ruleContext.Object as Item;
            if (item == null)
            {
                return string.Empty;
            }

            var parentItem = item.GetParent();
            if (parentItem == null)
            {
                return string.Empty;
            }

            return parentItem.TemplateName;
        }
    }
}
