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
            object languageObject;
            if (!parameters.TryGetValue("language", out languageObject))
            {
                throw new InvalidOperationException("Language missing");
            }

            var language = new Language(languageObject.ToString());
            var item = ruleContext.Object as Item;

            return item?.GetVersions(language).Count() ?? 0;
        }
    }
}
