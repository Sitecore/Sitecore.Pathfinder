using System.Collections.Generic;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions
{
    public class ItemNameCondition : StringConditionBase
    {
        public ItemNameCondition() : base("item-name")
        {
        }

        protected override IEnumerable<string> GetValues(IRuleContext ruleContext, IDictionary<string, string> parameters)
        {
            foreach (var obj in ruleContext.Objects)
            {
                var item = obj as DatabaseProjectItem;
                if (item == null)
                {
                    yield return null;
                    yield break;
                }

                yield return item.ItemName;
            }
        }
    }
}