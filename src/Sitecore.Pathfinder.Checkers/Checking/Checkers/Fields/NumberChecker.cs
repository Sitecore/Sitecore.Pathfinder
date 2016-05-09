// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers.Fields
{
    public class NumberChecker : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            foreach (var field in context.Project.Items.SelectMany(i => i.Fields).Where(f => string.Equals(f.TemplateField.Type, "Number", StringComparison.OrdinalIgnoreCase)))
            {
                if (string.IsNullOrEmpty(field.Value))
                {
                    continue;
                }

                int value;
                if (!int.TryParse(field.Value, out value))
                {
                    context.Trace.TraceWarning(Msg.C1057, "Number", TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), $"The field \"{field.FieldName}\" has a type of 'Number', but the value is not a valid number. Replace or remove the value.");
                }
            }
        }
    }
}
