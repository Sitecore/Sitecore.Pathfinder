using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions
{
    public class FieldValueCondition : StringConditionBase
    {
        public FieldValueCondition() : base("field-value")
        {
        }

        protected override IEnumerable<string> GetValues(IRuleContext ruleContext, IDictionary<string, string> parameters)
        {
            var fieldName = parameters.GetString("name");

            foreach (var obj in ruleContext.Objects)
            {
                var item = obj as Item;
                if (item == null)
                {
                    yield return null;
                    yield break;
                }

                var fields = item.Fields.Where(f => string.Equals(f.FieldName, fieldName, StringComparison.OrdinalIgnoreCase));
                foreach (var field in fields)
                {
                    yield return field.Value;
                }
            }
        }
    }
}