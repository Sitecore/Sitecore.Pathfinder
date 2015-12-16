// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.ItemConditions
{
    public class ItemVersionCountCondition : IntConditionBase
    {
        public ItemVersionCountCondition() : base("item-version-count")
        {
        }

        protected override int GetValue(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            object language;
            if (!parameters.TryGetValue("language", out language))
            {
                throw new InvalidOperationException("Language missing");
            }

            var item = ruleContext.Object as Item;
            return item == null ? 0 : item.GetVersions(language.ToString()).Count();
        }
    }
}
