using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.ItemConditions
{
    public class FieldCompiledValueCondition : StringConditionBase
    {
        public FieldCompiledValueCondition() : base("field-compiled-value")
        {
        }

        protected override string GetValue(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var item = ruleContext.Object as Item;
            if (item == null)
            {
                return string.Empty;
            }

            var fieldName = GetParameterValue(parameters, "name", ruleContext.Object);
            if (fieldName == null)
            {
                throw new ConfigurationException(Texts.Field_name_expected);
            }

            var field = item.Fields[fieldName];
            if (field == null)
            {
                return string.Empty;
            }

            return field.CompiledValue;
        }
    }
}