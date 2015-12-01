// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions
{
    public class ItemIdOrPath : StringConditionBase
    {
        public ItemIdOrPath() : base("item-id-or-path")
        {
        }

        protected override IEnumerable<string> GetValues(IRuleContext ruleContext, IDictionary<string, string> parameters)
        {
            var context = ruleContext as IItemBasesRuleContext;
            if (context == null)
            {
                yield break;
            }

            foreach (var itemBase in context.ItemBases)
            {
                yield return itemBase.ItemIdOrPath;
            }
        }
    }
}
