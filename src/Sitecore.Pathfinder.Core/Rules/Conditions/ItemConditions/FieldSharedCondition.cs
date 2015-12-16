using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.ItemConditions
{
    public class FieldSharedCondition : ConditionBase
    {
        public FieldSharedCondition() : base("is-field-shared")
        {
        }

        public override bool Evaluate(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var item = ruleContext.Object as Item;
            if (item == null)
            {
                return false;
            }

            var fieldName = GetParameterValue(parameters, "value", ruleContext.Object);
            if (fieldName == null)
            {
                throw new ConfigurationException(Texts.Field_name_expected);
            }

            var field = item.Fields[fieldName];
            if (field == null)
            {
                return false;
            }

            return field.TemplateField.Shared;
        }
    }
}