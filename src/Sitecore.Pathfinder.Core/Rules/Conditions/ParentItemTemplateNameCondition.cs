using System.Collections.Generic;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions
{
    public class ParentItemTemplateNameCondition : StringConditionBase
    {
        public ParentItemTemplateNameCondition() : base("parent-item-template-name")
        {
        }

        protected override IEnumerable<string> GetValues(IRuleContext ruleContext, IDictionary<string, string> parameters)
        {
            foreach (var obj in ruleContext.Objects)
            {
                var item = obj as Item;
                if (item == null)
                {
                    yield return null;
                    yield break;
                }

                var parentItem = item.GetParent();
                if (parentItem == null)
                {
                    yield return null;
                    yield break;
                }

                yield return parentItem.TemplateName;
            }
        }
    }
}