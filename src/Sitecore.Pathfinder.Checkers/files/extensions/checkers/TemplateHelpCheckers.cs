// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checkers
{
    public class TemplateHelpCheckers : Checker
    {
        [Export("Check")]
        public IEnumerable<Diagnostic> TemplateFieldLongHelpShouldEndWithDot(ICheckerContext context)
        {
            return from template in context.Project.Templates
                from field in template.Fields
                where !string.IsNullOrEmpty(field.LongHelp) && !field.LongHelp.EndsWith(".")
                select Warning(Msg.C1018, "Template field long help text should end with '.'", TraceHelper.GetTextNode(field.LongHelpProperty, field), field.FieldName);
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> TemplateFieldLongHelpShouldStartWithCapitalLetter(ICheckerContext context)
        {
            return from template in context.Project.Templates
                from field in template.Fields
                where !string.IsNullOrEmpty(field.LongHelp) && !char.IsUpper(template.LongHelp[0])
                select Warning(Msg.C1018, "Template field long help text should start with a capital letter", TraceHelper.GetTextNode(field.LongHelpProperty, field), field.FieldName);
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> TemplateFieldShortHelpShouldEndWithDot(ICheckerContext context)
        {
            return from template in context.Project.Templates
                from field in template.Fields
                where !string.IsNullOrEmpty(field.ShortHelp) && !field.ShortHelp.EndsWith(".")
                select Warning(Msg.C1018, "Template field short help text should end with '.'", TraceHelper.GetTextNode(field.ShortHelpProperty, field), field.FieldName);
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> TemplateFieldShortHelpShouldStartWithCapitalLetter(ICheckerContext context)
        {
            return from template in context.Project.Templates
                from field in template.Fields
                where !string.IsNullOrEmpty(field.ShortHelp) && !char.IsUpper(template.ShortHelp[0])
                select Warning(Msg.C1018, "Template field short help text should start with a capital letter", TraceHelper.GetTextNode(field.ShortHelpProperty, field), field.FieldName);
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> TemplateFieldShouldHaveLongHelp(ICheckerContext context)
        {
            return from template in context.Project.Templates
                from field in template.Fields
                where string.IsNullOrEmpty(template.LongHelp)
                select Warning(Msg.C1017, "Template field should have a long help text", TraceHelper.GetTextNode(field.LongHelpProperty, field), field.FieldName);
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> TemplateFieldShouldHaveShortHelp(ICheckerContext context)
        {
            return from template in context.Project.Templates
                from field in template.Fields
                where string.IsNullOrEmpty(template.ShortHelp)
                select Warning(Msg.C1017, "Template field should have a short help text", TraceHelper.GetTextNode(field.ShortHelpProperty, field), field.FieldName);
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> TemplateLongHelpShouldEndWithDot(ICheckerContext context)
        {
            return from template in context.Project.Templates
                where !string.IsNullOrEmpty(template.LongHelp) && !template.LongHelp.EndsWith(".")
                select Warning(Msg.C1018, "Template long help text should end with '.'", TraceHelper.GetTextNode(template), template.ItemName);
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> TemplateLongHelpShouldStartWithCapitalLetter(ICheckerContext context)
        {
            return from template in context.Project.Templates
                where !string.IsNullOrEmpty(template.LongHelp) && !char.IsUpper(template.LongHelp[0])
                select Warning(Msg.C1019, "Template long help text should start with a capital letter", TraceHelper.GetTextNode(template), template.ItemName);
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> TemplateShortHelpShouldEndWithDot(ICheckerContext context)
        {
            return from template in context.Project.Templates
                where !string.IsNullOrEmpty(template.ShortHelp) && !template.ShortHelp.EndsWith(".")
                select Warning(Msg.C1015, "Template short help text should end with '.'", TraceHelper.GetTextNode(template), template.ItemName);
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> TemplateShortHelpShouldStartWithCapitalLetter(ICheckerContext context)
        {
            return from template in context.Project.Templates
                where !string.IsNullOrEmpty(template.ShortHelp) && !char.IsUpper(template.ShortHelp[0])
                select Warning(Msg.C1016, "Template short help text should start with a capital letter", TraceHelper.GetTextNode(template), template.ItemName);
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> TemplateShouldHaveLongHelp(ICheckerContext context)
        {
            return from template in context.Project.Templates
                where string.IsNullOrEmpty(template.LongHelp)
                select Warning(Msg.C1017, "Template should have a long help text", TraceHelper.GetTextNode(template), template.ItemName);
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> TemplateShouldHaveShortHelp(ICheckerContext context)
        {
            return from template in context.Project.Templates
                where string.IsNullOrEmpty(template.ShortHelp)
                select Warning(Msg.C1014, "Template should have a short help text", TraceHelper.GetTextNode(template), template.ItemName);
        }
    }
}
