using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions
{
    public class DatabaseNameCondition : StringConditionBase
    {
        public DatabaseNameCondition() : base("database-name")
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

                yield return item.DatabaseName;
            }
        }
    }
}