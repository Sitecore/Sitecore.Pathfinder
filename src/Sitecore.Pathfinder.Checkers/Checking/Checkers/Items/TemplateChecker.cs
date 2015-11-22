// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers.Items
{
    public class TemplateChecker : CheckerBase
    {
        public TemplateChecker() : base("Template checker", Templates)
        {
        }

        public override void Check(ICheckerContext context)
        {
            foreach (var template in context.Project.ProjectItems.OfType<Template>().Where(i => !i.IsImport))
            {
                CheckTemplate(context, template);
            }
        }

        private void CheckGoodName([NotNull] ICheckerContext context, [NotNull] SourceProperty<string> itemName)
        {
            if (itemName.GetValue().IndexOf(' ') >= 0)
            {
                context.Trace.TraceWarning("Name should not contain spaces", TraceHelper.GetTextNode(itemName), itemName.GetValue());
            }
        }

        private void CheckTemplate([NotNull] ICheckerContext context, [NotNull] Template template)
        {
            CheckGoodName(context, template.ItemNameProperty);

            if (!template.Sections.Any() && template.BaseTemplates == Constants.Templates.StandardTemplate)
            {
                context.Trace.TraceWarning("Empty templates should be avoided. Consider using the Folder template instead", TraceHelper.GetTextNode(template), template.ItemName);
            }

            if (string.IsNullOrEmpty(template.ShortHelp))
            {
                context.Trace.TraceWarning("Template should have a short help text", TraceHelper.GetTextNode(template), template.ItemName);
            }

            if (!string.IsNullOrEmpty(template.ShortHelp) && !template.ShortHelp.EndsWith("."))
            {
                context.Trace.TraceWarning("Template short help text should end with '.'", TraceHelper.GetTextNode(template), template.ItemName);
            }

            if (!string.IsNullOrEmpty(template.ShortHelp) && !char.IsUpper(template.ShortHelp[0]))
            {
                context.Trace.TraceWarning("Template short help text should end with a capital letter", TraceHelper.GetTextNode(template), template.ItemName);
            }

            if (string.IsNullOrEmpty(template.LongHelp))
            {
                context.Trace.TraceWarning("Template should should have a long help text", TraceHelper.GetTextNode(template), template.ItemName);
            }

            if (!string.IsNullOrEmpty(template.LongHelp) && !template.LongHelp.EndsWith("."))
            {
                context.Trace.TraceWarning("Template long help text should end with '.'", TraceHelper.GetTextNode(template), template.ItemName);
            }

            if (!string.IsNullOrEmpty(template.LongHelp) && !char.IsUpper(template.LongHelp[0]))
            {
                context.Trace.TraceWarning("Template long help text should end with a capital letter", TraceHelper.GetTextNode(template), template.ItemName);
            }

            if (string.IsNullOrEmpty(template.Icon))
            {
                context.Trace.TraceWarning("Template should should have an icon", TraceHelper.GetTextNode(template), template.ItemName);
            }

            foreach (var templateSection in template.Sections)
            {
                CheckTemplateSection(context, templateSection);
            }
        }

        private void CheckTemplateField([NotNull] ICheckerContext context, [NotNull] TemplateField field)
        {
            CheckGoodName(context, field.FieldNameProperty);

            if (string.IsNullOrEmpty(field.ShortHelp))
            {
                context.Trace.TraceWarning("Template field should have a short help text", TraceHelper.GetTextNode(field.ShortHelpProperty, field), field.FieldName);
            }

            if (!string.IsNullOrEmpty(field.ShortHelp) && !field.ShortHelp.EndsWith("."))
            {
                context.Trace.TraceWarning("Template field short help text should end with '.'", TraceHelper.GetTextNode(field.ShortHelpProperty, field), field.FieldName);
            }

            if (!string.IsNullOrEmpty(field.ShortHelp) && !char.IsUpper(field.ShortHelp[0]))
            {
                context.Trace.TraceWarning("Template field short help text should end with a capital letter", TraceHelper.GetTextNode(field.ShortHelpProperty, field), field.FieldName);
            }

            if (string.IsNullOrEmpty(field.LongHelp))
            {
                context.Trace.TraceWarning("Template field should should have a long help text", TraceHelper.GetTextNode(field.LongHelpProperty, field), field.FieldName);
            }

            if (!string.IsNullOrEmpty(field.LongHelp) && !field.LongHelp.EndsWith("."))
            {
                context.Trace.TraceWarning("Template field long help text should end with '.'", TraceHelper.GetTextNode(field.LongHelpProperty, field), field.FieldName);
            }

            if (!string.IsNullOrEmpty(field.LongHelp) && !char.IsUpper(field.LongHelp[0]))
            {
                context.Trace.TraceWarning("Template field long help text should end with a capital letter", TraceHelper.GetTextNode(field.LongHelpProperty, field), field.FieldName);
            }
        }

        private void CheckTemplateSection([NotNull] ICheckerContext context, [NotNull] TemplateSection templateSection)
        {
            CheckGoodName(context, templateSection.SectionNameProperty);

            if (!templateSection.Fields.Any())
            {
                context.Trace.TraceWarning("Template section is empty", TraceHelper.GetTextNode(templateSection), templateSection.SectionName);
            }

            foreach (var field in templateSection.Fields)
            {
                CheckTemplateField(context, field);
            }
        }
    }
}
