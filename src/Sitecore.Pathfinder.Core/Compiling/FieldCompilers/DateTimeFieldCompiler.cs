// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using System.Globalization;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    [Export(typeof(IFieldCompiler)), Shared]
    public class DateTimeFieldCompiler : FieldCompilerBase
    {
        public DateTimeFieldCompiler() : base(Constants.FieldCompilers.Normal)
        {
        }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            return string.Equals(field.TemplateField.Type, "datetime", StringComparison.OrdinalIgnoreCase);
        }

        public override string Compile(IFieldCompileContext context, Field field)
        {
            var value = field.Value.Trim();
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            if (value == "00010101T000000")
            {
                return value;
            }

            // check if value is an ISO date
            if (value.FromIsoToDateTime() != DateTime.MinValue)
            {
                return value;
            }

            DateTime dateTime;
            if (!DateTime.TryParse(value, context.Culture, DateTimeStyles.None, out dateTime))
            {
                context.Trace.TraceError(Msg.C1058, Texts.DateTime_field_must_contain_a_valid_date_time_value, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty), value);
                return string.Empty;
            }

            return dateTime.ToString(@"yyyyMMddTHHmmss") + "Z";
        }
    }
}
