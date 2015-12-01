// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions
{
    public class TemplateName : StringConditionBase
    {
        public TemplateName() : base("template-name")
        {
        }

        protected override IEnumerable<string> GetValues(IRuleContext ruleContext, IDictionary<string, string> parameters)
        {
            var context = ruleContext as IItemsRuleContext;
            if (context == null)
            {
                yield break;
            }

            foreach (var item in context.Items)
            {
                yield return item.TemplateName;
            }
        }
    }
}
