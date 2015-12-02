using System.Collections.Generic;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions
{
    public class QualifiedNameCondition : StringConditionBase
    {
        public QualifiedNameCondition() : base("qualified-name")
        {
        }

        protected override IEnumerable<string> GetValues(IRuleContext ruleContext, IDictionary<string, string> parameters)
        {
            foreach (var obj in ruleContext.Objects)
            {
                var item = obj as IProjectItem;
                if (item == null)
                {
                    yield return null;
                    yield break;
                }

                yield return item.QualifiedName;
            }
        }
    }
}