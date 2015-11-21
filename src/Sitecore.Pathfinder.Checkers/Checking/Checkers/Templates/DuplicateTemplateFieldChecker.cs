// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers.Templates
{
    public class DuplicateTemplateFieldChecker : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            foreach (var template in context.Project.Templates)
            {
                var fields = template.GetAllFields().ToArray();

                for (var i0 = 0; i0 < fields.Length - 2; i0++)
                {
                    var field0 = fields[i0];

                    for (var i1 = i0 + 1; i1 < fields.Length - 1; i1++)
                    {
                        var field1 = fields[i1];

                        if (string.Compare(field0.FieldName, field1.FieldName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            context.Trace.TraceWarning("Duplicate template field name", TraceHelper.GetTextNode(field0.FieldNameProperty, field1.FieldNameProperty, field0, field1), 
                                $"The template contains two or more field with the same name \"{field0.FieldName}\". Even if these fields are located in different sections, it is still not recommended as the name is ambiguous. Rename one or more of the fields.");
                        }
                    }
                }
            }
        }
    }
}
