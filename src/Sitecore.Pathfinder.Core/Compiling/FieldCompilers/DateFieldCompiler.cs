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
                return string.Empty;
            }

            return dateTime.ToString(@"yyyyMMdd") + "T000000Z";
        }
    }
}
