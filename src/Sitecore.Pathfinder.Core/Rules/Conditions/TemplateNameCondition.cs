// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions
{
    public class TemplateNameCondition : StringConditionBase
    {
        public TemplateNameCondition() : base("template-name")
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

                yield return item.TemplateName;
            }
        }
    }
}
