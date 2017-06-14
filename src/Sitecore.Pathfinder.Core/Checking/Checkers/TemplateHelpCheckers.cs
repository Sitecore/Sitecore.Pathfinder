// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    [Export(typeof(IChecker)), Shared]
    public class TemplateHelpCheckers : Checker
    {
        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> TemplateFieldLongHelpShouldEndWithDot([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                from field in template.Fields
                where !string.IsNullOrEmpty(field.LongHelp) && !field.LongHelp.EndsWith(".")
                select Warning(context, Msg.C1018, "Template field long help text should end with '.'", TraceHelper.GetTextNode(field.LongHelpProperty, field), field.FieldName);
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> TemplateFieldLongHelpShouldStartWithCapitalLetter([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                from field in template.Fields
                where !string.IsNullOrEmpty(field.LongHelp) && !char.IsUpper(template.LongHelp[0])
                select Warning(context, Msg.C1018, "Template field long help text should start with a capital letter", TraceHelper.GetTextNode(field.LongHelpProperty, field), field.FieldName);
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> TemplateFieldShortHelpShouldEndWithDot([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                from field in template.Fields
                where !string.IsNullOrEmpty(field.ShortHelp) && !field.ShortHelp.EndsWith(".")
                select Warning(context, Msg.C1018, "Template field short help text should end with '.'", TraceHelper.GetTextNode(field.ShortHelpProperty, field), field.FieldName);
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> TemplateFieldShortHelpShouldStartWithCapitalLetter([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                from field in template.Fields
                where !string.IsNullOrEmpty(field.ShortHelp) && !char.IsUpper(template.ShortHelp[0])
                select Warning(context, Msg.C1018, "Template field short help text should start with a capital letter", TraceHelper.GetTextNode(field.ShortHelpProperty, field), field.FieldName);
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> TemplateFieldShouldHaveLongHelp([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                from field in template.Fields
                where string.IsNullOrEmpty(template.LongHelp)
                select Warning(context, Msg.C1017, "Template field should have a long help text", TraceHelper.GetTextNode(field.LongHelpProperty, field), field.FieldName);
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> TemplateFieldShouldHaveShortHelp([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                from field in template.Fields
                where string.IsNullOrEmpty(template.ShortHelp)
                select Warning(context, Msg.C1017, "Template field should have a short help text", TraceHelper.GetTextNode(field.ShortHelpProperty, field), field.FieldName);
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> TemplateLongHelpShouldEndWithDot([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where !string.IsNullOrEmpty(template.LongHelp) && !template.LongHelp.EndsWith(".")
                select Warning(context, Msg.C1018, "Template long help text should end with '.'", TraceHelper.GetTextNode(template), template.ItemName);
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> TemplateLongHelpShouldStartWithCapitalLetter([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where !string.IsNullOrEmpty(template.LongHelp) && !char.IsUpper(template.LongHelp[0])
                select Warning(context, Msg.C1019, "Template long help text should start with a capital letter", TraceHelper.GetTextNode(template), template.ItemName);
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> TemplateShortHelpShouldEndWithDot([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where !string.IsNullOrEmpty(template.ShortHelp) && !template.ShortHelp.EndsWith(".")
                select Warning(context, Msg.C1015, "Template short help text should end with '.'", TraceHelper.GetTextNode(template), template.ItemName);
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> TemplateShortHelpShouldStartWithCapitalLetter([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where !string.IsNullOrEmpty(template.ShortHelp) && !char.IsUpper(template.ShortHelp[0])
                select Warning(context, Msg.C1016, "Template short help text should start with a capital letter", TraceHelper.GetTextNode(template), template.ItemName);
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> TemplateShouldHaveLongHelp([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where string.IsNullOrEmpty(template.LongHelp)
                select Warning(context, Msg.C1017, "Template should have a long help text", TraceHelper.GetTextNode(template), template.ItemName);
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> TemplateShouldHaveShortHelp([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where string.IsNullOrEmpty(template.ShortHelp)
                select Warning(context, Msg.C1014, "Template should have a short help text", TraceHelper.GetTextNode(template), template.ItemName);
        }
    }
}
