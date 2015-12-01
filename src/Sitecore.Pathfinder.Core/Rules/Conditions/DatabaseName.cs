using System.Collections.Generic;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions
{
    public class DatabaseName : StringConditionBase
    {
        public DatabaseName() : base("database-name")
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
                yield return itemBase.DatabaseName;
            }
        }
    }
}