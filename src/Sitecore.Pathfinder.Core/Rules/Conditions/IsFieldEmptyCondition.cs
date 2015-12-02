using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions
{
    public class IsFieldEmptyCondition : ConditionBase
    {
        public IsFieldEmptyCondition() : base("is-field-empty")
        {
        }

        public override bool Evaluate(IRuleContext ruleContext, IDictionary<string, string> parameters)
        {
            if (!ruleContext.Objects.Any())
            {
                return true;
            }

            var fieldName = parameters.GetString("value");

            foreach (var obj in ruleContext.Objects)
            {
                var item = obj as Item;
                if (item == null)
                {
                    return false;
                }

                var fields = item.Fields.Where(f => string.Equals(f.FieldName, fieldName, StringComparison.OrdinalIgnoreCase));
                if (fields.Any(f => !string.IsNullOrEmpty(f.Value)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}