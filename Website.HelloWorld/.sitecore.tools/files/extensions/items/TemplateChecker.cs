// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Checking.Checkers.Items
{
    [Export(typeof(IChecker))]
    public class TemplateChecker : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            foreach (var template in context.Project.Items.OfType<Template>())
            {
                CheckTemplate(context, template);
            }
        }

        private void CheckGoodName([NotNull] ICheckerContext context, [NotNull] Attribute<string> itemName)
        {
            if (itemName.Value.IndexOf(' ') >= 0)
            {
                context.Trace.TraceWarning("Name should not contain spaces", itemName.Source ?? TextNode.Empty, itemName.Value);
            }
        }

        private void CheckTemplate([NotNull] ICheckerContext context, [NotNull] Template template)
        {
            CheckGoodName(context, template.ItemName);

            if (!template.Sections.Any() && template.BaseTemplates == Constants.Templates.StandardTemplate)
            {
                context.Trace.TraceWarning("Empty templates should be avoided. Consider using the Folder template instead", template.ItemName.Source ?? TextNode.Empty, template.ItemName.Value);
            }

            if (string.IsNullOrEmpty(template.ShortHelp))
            {
                context.Trace.TraceWarning("Template should have a short help text", template.ItemName.Source ?? TextNode.Empty, template.ItemName.Value);
            }

            if (!string.IsNullOrEmpty(template.ShortHelp) && !template.ShortHelp.EndsWith("."))
            {
                context.Trace.TraceWarning("Template short help text should end with '.'", template.ItemName.Source ?? TextNode.Empty, template.ItemName.Value);
            }

            if (!string.IsNullOrEmpty(template.ShortHelp) && !char.IsUpper(template.ShortHelp[0]))
            {
                context.Trace.TraceWarning("Template short help text should end with a capital letter", template.ItemName.Source ?? TextNode.Empty, template.ItemName.Value);
            }

            if (string.IsNullOrEmpty(template.LongHelp))
            {
                context.Trace.TraceWarning("Template should should have a long help text", template.ItemName.Source ?? TextNode.Empty, template.ItemName.Value);
            }

            if (!string.IsNullOrEmpty(template.LongHelp) && !template.LongHelp.EndsWith("."))
            {
                context.Trace.TraceWarning("Template long help text should end with '.'", template.ItemName.Source ?? TextNode.Empty, template.ItemName.Value);
            }

            if (!string.IsNullOrEmpty(template.LongHelp) && !char.IsUpper(template.LongHelp[0]))
            {
                context.Trace.TraceWarning("Template long help text should end with a capital letter", template.ItemName.Source ?? TextNode.Empty, template.ItemName.Value);
            }

            if (string.IsNullOrEmpty(template.Icon.Value))
            {
                context.Trace.TraceWarning("Template should should have an icon", template.ItemName.Source ?? TextNode.Empty, template.ItemName.Value);
            }

            foreach (var templateSection in template.Sections)
            {
                CheckTemplateSection(context, templateSection);
            }
        }

        private void CheckTemplateField([NotNull] ICheckerContext context, [NotNull] TemplateField field)
        {
            CheckGoodName(context, field.FieldName);

            if (string.IsNullOrEmpty(field.ShortHelp))
            {
                context.Trace.TraceWarning("Template field should have a short help text", field.FieldName.Source ?? TextNode.Empty, field.FieldName.Value);
            }

            if (!string.IsNullOrEmpty(field.ShortHelp) && !field.ShortHelp.EndsWith("."))
            {
                context.Trace.TraceWarning("Template field short help text should end with '.'", field.FieldName.Source ?? TextNode.Empty, field.FieldName.Value);
            }

            if (!string.IsNullOrEmpty(field.ShortHelp) && !char.IsUpper(field.ShortHelp[0]))
            {
                context.Trace.TraceWarning("Template field short help text should end with a capital letter", field.FieldName.Source ?? TextNode.Empty, field.FieldName.Value);
            }

            if (string.IsNullOrEmpty(field.LongHelp))
            {
                context.Trace.TraceWarning("Template field should should have a long help text", field.FieldName.Source ?? TextNode.Empty, field.FieldName.Value);
            }

            if (!string.IsNullOrEmpty(field.LongHelp) && !field.LongHelp.EndsWith("."))
            {
                context.Trace.TraceWarning("Template field long help text should end with '.'", field.FieldName.Source ?? TextNode.Empty, field.FieldName.Value);
            }

            if (!string.IsNullOrEmpty(field.LongHelp) && !char.IsUpper(field.LongHelp[0]))
            {
                context.Trace.TraceWarning("Template field long help text should end with a capital letter", field.FieldName.Source ?? TextNode.Empty, field.FieldName.Value);
            }
        }

        private void CheckTemplateSection([NotNull] ICheckerContext context, [NotNull] TemplateSection templateSection)
        {
            CheckGoodName(context, templateSection.SectionName);

            if (!templateSection.Fields.Any())
            {
                context.Trace.TraceWarning("Template section is empty", templateSection.TemplateSectionTextNode, templateSection.SectionName.Value);
            }

            foreach (var field in templateSection.Fields)
            {
                CheckTemplateField(context, field);
            }
        }
    }
}
