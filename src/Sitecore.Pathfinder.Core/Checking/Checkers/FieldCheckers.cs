using System;
using System.Collections.Generic;
using System.Composition;
using System.Globalization;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;
using Version = Sitecore.Pathfinder.Projects.Items.Version;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    [Export(typeof(IChecker)), Shared]
    public class FieldCheckers : Checker
    {
        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> CheckboxMustBeTrueOrFalse([NotNull] ICheckerContext context)
        {
            return from field in context.Project.Items.SelectMany(i => i.Fields).Where(f => string.Equals(f.TemplateField.Type, "Checkbox", StringComparison.OrdinalIgnoreCase))
                where !string.Equals(field.Value, "true", StringComparison.OrdinalIgnoreCase) && !string.Equals(field.Value, "false", StringComparison.OrdinalIgnoreCase)
                select Error(context, Msg.C1066, Texts.Checkbox_field_value_must_be__true__or__false__, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), details: $"The field \"{field.FieldName}\" has a type of 'Checkbox', but the value is not a valid boolean. Set the value to 'true' or 'false'.");
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> DateCannotBeParsed([NotNull] ICheckerContext context)
        {
            DateTime dateTime;
            return from field in context.Project.Items.SelectMany(i => i.Fields).Where(f => string.Equals(f.TemplateField.Type, "Date", StringComparison.OrdinalIgnoreCase))
                where !string.IsNullOrEmpty(field.Value) && field.Value != "00010101T000000" && field.Value.FromIsoToDateTime() == DateTime.MinValue
                where !DateTime.TryParse(field.Value, context.Culture, DateTimeStyles.None, out dateTime)
                select Warning(context, Msg.C1133, "Date cannot be parsed", TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), $"The field \"{field.FieldName}\" has a type of 'Date', but the value is not a valid date. Replace or remove the value.");
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> DateIsNotValid([NotNull] ICheckerContext context)
        {
            return from field in context.Project.Items.SelectMany(i => i.Fields).Where(f => string.Equals(f.TemplateField.Type, "Date", StringComparison.OrdinalIgnoreCase))
                where !string.IsNullOrEmpty(field.Value)
                let dateTime = field.Value.FromIsoToDateTime(DateTime.MaxValue)
                where dateTime == DateTime.MaxValue
                select Warning(context, Msg.C1066, "Date is not valid", TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), $"The field \"{field.FieldName}\" has a type of 'Date', but the value is not a valid date. Replace or remove the value.");
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> DateTimeCannotBeParsed([NotNull] ICheckerContext context)
        {
            DateTime dateTime;
            return from field in context.Project.Items.SelectMany(i => i.Fields).Where(f => string.Equals(f.TemplateField.Type, "Datetime", StringComparison.OrdinalIgnoreCase))
                where !string.IsNullOrEmpty(field.Value) && field.Value != "00010101T000000" && field.Value.FromIsoToDateTime() == DateTime.MinValue
                where !DateTime.TryParse(field.Value, context.Culture, DateTimeStyles.None, out dateTime)
                select Warning(context, Msg.C1058, "Datetime cannot be parsed", TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), $"The field \"{field.FieldName}\" has a type of 'Datetime', but the value is not a valid date. Replace or remove the value.");
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> DateTimeIsNotValid([NotNull] ICheckerContext context)
        {
            return from field in context.Project.Items.SelectMany(i => i.Fields).Where(f => string.Equals(f.TemplateField.Type, "Datetime", StringComparison.OrdinalIgnoreCase))
                where !string.IsNullOrEmpty(field.Value)
                let dateTime = field.Value.FromIsoToDateTime(DateTime.MaxValue)
                where dateTime == DateTime.MaxValue
                select Warning(context, Msg.C1067, "Datetime is not valid", TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), $"The field \"{field.FieldName}\" has a type of 'Datetime', but the value is not a valid date. Replace or remove the value.");
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> FieldContainsLoremIpsum([NotNull] ICheckerContext context)
        {
            return from field in context.Project.Items.SelectMany(i => i.Fields)
                where field.Value.IndexOf("Lorem Ipsum", StringComparison.OrdinalIgnoreCase) >= 0
                select Warning(context, Msg.C1008, "Field contains 'Lorem Ipsum' text", TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), $"The field \"{field.FieldName}\" contains the test data text: \"Lorem Ipsum...\". Replace or remove the text data.");
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> FieldSharing([NotNull] ICheckerContext context)
        {
            foreach (var field in context.Project.Items.SelectMany(i => i.Fields))
            {
                if (field.TemplateField == TemplateField.Empty)
                {
                    continue;
                }

                if (field.TemplateField.Shared)
                {
                    if (field.Language != Language.Empty && field.Language != Language.Undefined)
                    {
                        yield return Warning(context, Msg.P1028, "Field is shared, but is specified in the language", field.SourceTextNode, field.FieldName);
                    }

                    if (field.Version != Version.Undefined)
                    {
                        yield return Warning(context, Msg.P1029, "Field is shared, but has a version", field.SourceTextNode, field.FieldName);
                    }
                }
                else if (field.TemplateField.Unversioned)
                {
                    if (field.Language == Language.Empty || field.Language == Language.Undefined)
                    {
                        yield return Warning(context, Msg.P1030, "Field is unversioned, but no language is specified", field.SourceTextNode, field.FieldName);
                    }

                    if (field.Version != Version.Undefined)
                    {
                        yield return Warning(context, Msg.P1031, "Field is unversioned, but has a version", field.SourceTextNode, field.FieldName);
                    }
                }
                else
                {
                    if (field.Language == Language.Empty || field.Language == Language.Undefined)
                    {
                        yield return Warning(context, Msg.P1032, "Field is versioned, but no language is specified", field.SourceTextNode, field.FieldName);
                    }

                    if (field.Version == Version.Undefined)
                    {
                        yield return Warning(context, Msg.P1033, "Field is versioned, but no version is specified", field.SourceTextNode, field.FieldName);
                    }
                }
            }
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> NumberIsNotValid([NotNull] ICheckerContext context)
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
                    yield return Warning(context, Msg.C1057, "Number is not valid", TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), $"The field \"{field.FieldName}\" has a type of 'Number', but the value is not a valid number. Replace or remove the value.");
                }
            }
        }
    }
}
