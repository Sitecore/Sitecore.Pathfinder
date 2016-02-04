// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers.Fields
{
    public class LorumIpsumChecker : CheckerBase
    {
        public LorumIpsumChecker() : base("Field containing Lorem Ipsum", Fields)
        {
        }

        public override void Check(ICheckerContext context)
        {
            foreach (var field in context.Project.Items.SelectMany(i => i.Fields))
            {
                if (field.Value.IndexOf("Lorem Ipsum", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    context.Trace.TraceWarning(Msg.C1008, "Lorem Ipsum text", TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), $"The field \"{field.FieldName}\" contains the test data text: \"Lorem Ipsum...\". Replace or remove the text data.");
                }
            }
        }
    }
}
