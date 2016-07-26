// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Globalization;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    public class DateFieldCompiler : FieldCompilerBase
    {
        public DateFieldCompiler() : base(Constants.FieldCompilers.Normal)
        {
        }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            return string.Equals(field.TemplateField.Type, "date", StringComparison.OrdinalIgnoreCase);
        }

        public override string Compile(IFieldCompileContext context, Field field)
        {
            var value = field.Value.Trim();
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            // check if value is an ISO date
            if (value.FromIsoToDateTime() != DateTime.MinValue)
            {
                return value;
            }

            DateTime dateTime;
            if (!DateTime.TryParse(value, context.Culture, DateTimeStyles.None, out dateTime))
            {
                context.Trace.TraceError(Msg.C1058, Texts.Date_field_must_contain_a_valid_date_value, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty), value);
                return string.Empty;
            }

            return dateTime.ToString(@"yyyyMMdd") + "T000000Z";
        }
    }
}
