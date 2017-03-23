// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    [Export(typeof(IChecker)), Shared]
    public class FieldCheckers : Checker
    {
        [ItemNotNull, NotNull, Check]
        public IEnumerable<Diagnostic> DateIsNotValid([NotNull] ICheckerContext context)
        {
            return from field in context.Project.Items.SelectMany(i => i.Fields).Where(f => string.Equals(f.TemplateField.Type, "Date", StringComparison.OrdinalIgnoreCase))
                where !string.IsNullOrEmpty(field.Value)
                let dateTime = field.Value.FromIsoToDateTime(DateTime.MaxValue)
                where dateTime == DateTime.MaxValue
                select Warning(Msg.C1066, "Date is not valid", TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), $"The field \"{field.FieldName}\" has a type of 'Date', but the value is not a valid date. Replace or remove the value.");
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<Diagnostic> DateTimeIsNotValid([NotNull] ICheckerContext context)
        {
            return from field in context.Project.Items.SelectMany(i => i.Fields).Where(f => string.Equals(f.TemplateField.Type, "Datetime", StringComparison.OrdinalIgnoreCase))
                where !string.IsNullOrEmpty(field.Value)
                let dateTime = field.Value.FromIsoToDateTime(DateTime.MaxValue)
                where dateTime == DateTime.MaxValue
                select Warning(Msg.C1067, "Datetime is not valid", TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), $"The field \"{field.FieldName}\" has a type of 'Datetime', but the value is not a valid date. Replace or remove the value.");
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<Diagnostic> FieldContainsLoremIpsum([NotNull] ICheckerContext context)
        {
            return from field in context.Project.Items.SelectMany(i => i.Fields)
                where field.Value.IndexOf("Lorem Ipsum", StringComparison.OrdinalIgnoreCase) >= 0
                select Warning(Msg.C1008, "Field contains 'Lorem Ipsum' text", TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), $"The field \"{field.FieldName}\" contains the test data text: \"Lorem Ipsum...\". Replace or remove the text data.");
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<Diagnostic> NumberIsNotValid([NotNull] ICheckerContext context)
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
                    yield return Warning(Msg.C1057, "Number is not valid", TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), $"The field \"{field.FieldName}\" has a type of 'Number', but the value is not a valid number. Replace or remove the value.");
                }
            }
        }
    }
}
