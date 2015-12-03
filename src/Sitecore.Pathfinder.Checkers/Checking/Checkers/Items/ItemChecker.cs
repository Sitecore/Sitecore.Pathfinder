// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers.Items
{
    public class ItemChecker : CheckerBase
    {
        public ItemChecker() : base("Item Checker", Items + Fields)
        {
        }

        public override void Check(ICheckerContext context)
        {
            foreach (var item in context.Project.Items)
            {
                CheckTemplate(context, item);
            }
        }

        protected virtual void CheckGoodName([NotNull] ICheckerContext context, [NotNull] SourceProperty<string> itemName)
        {
            var value = itemName.GetValue();
            if (value.IndexOf(' ') >= 0 && value != "__Standard Values")
            {
                context.Trace.TraceWarning(Msg.C1003, "Name should not contain spaces", TraceHelper.GetTextNode(itemName), value);
            }
        }

        protected virtual void CheckTemplate([NotNull] ICheckerContext context, [NotNull] Item item)
        {
            CheckGoodName(context, item.ItemNameProperty);
            CheckTemplateFields(context, item);
        }

        protected virtual void CheckTemplateFields([NotNull] ICheckerContext context, [NotNull] Item item)
        {
            var template = item.Template;
            if (template == Template.Empty)
            {
                if (item.Project.FindQualifiedItem<IProjectItem>(item.TemplateIdOrPath) == null)
                {
                    context.Trace.TraceWarning(Msg.C1004, "Item template not found", TraceHelper.GetTextNode(item.TemplateIdOrPathProperty, item, item.ItemNameProperty), item.TemplateIdOrPath);
                }

                return;
            }

            var templateFields = template.GetAllFields().ToList();

            foreach (var field in item.Fields)
            {
                var templateField = templateFields.FirstOrDefault(f => string.Equals(f.FieldName, field.FieldName, StringComparison.OrdinalIgnoreCase));
                if (templateField == null)
                {
                    context.Trace.TraceWarning(Msg.C1005, "Field is not defined in the template", TraceHelper.GetTextNode(field.FieldNameProperty, field.Item), field.FieldName);
                }
            }
        }
    }
}
