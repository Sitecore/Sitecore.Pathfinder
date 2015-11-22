// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers.Templates
{
    public class FieldIdTemplateFieldIdChecker : CheckerBase
    {
        public FieldIdTemplateFieldIdChecker() : base("Field ID and Template Field ID must match", Fields + TemplateFields)
        {
        }

        public override void Check(ICheckerContext context)
        {
            foreach (var item in context.Project.Items)
            {
                foreach (var field in item.Fields)
                {
                    var templateField = field.TemplateField;
                    if (templateField == TemplateField.Empty)
                    {
                        continue;
                    }

                    if (templateField.Uri.Guid != field.FieldId)
                    {
                        context.Trace.TraceWarning(Msg.C1024, "Field ID and Template Field ID differs", TraceHelper.GetTextNode(field.FieldIdProperty, field), $"FieldId: {field.FieldId.Format()}, TemplateFieldId: {templateField.Uri.Guid.Format()}");
                    }
                }
            }
        }
    }
}
