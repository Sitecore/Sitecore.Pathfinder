using System.Collections.Generic;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions
{
    public class ShortName : StringConditionBase
    {
        public ShortName() : base("short-name")
        {
        }

        protected override IEnumerable<string> GetValues(IRuleContext ruleContext, IDictionary<string, string> parameters)
        {
            var context = ruleContext as IProjectItemRuleContext;
            if (context == null)
            {
                yield break;
            }

            foreach (var projectItem in context.ProjectItems)
            {
                yield return projectItem.ShortName;
            }
        }
    }
}