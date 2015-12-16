using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.ItemConditions
{
    public class IsFieldEmptyCondition : ConditionBase
    {
        public IsFieldEmptyCondition() : base("is-field-empty")
        {
        }

        public override bool Evaluate(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var fieldName = GetParameterValue(parameters, "value", ruleContext.Object);
            if (fieldName == null)
            {
                throw new ConfigurationException(Texts.Field_name_expected);
            }

            var item = ruleContext.Object as Item;
            if (item == null)
            {
                return false;
            }

            var fields = item.Fields.Where(f => string.Equals(f.FieldName, fieldName, StringComparison.OrdinalIgnoreCase));
            if (fields.Any(f => !string.IsNullOrEmpty(f.Value)))
            {
                return false;
            }

            return true;
        }
    }
}