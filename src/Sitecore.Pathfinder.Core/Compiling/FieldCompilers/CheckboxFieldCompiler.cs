// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    [Export(typeof(IFieldCompiler)), Shared]
    public class CheckboxFieldCompiler : FieldCompilerBase
    {
        public CheckboxFieldCompiler() : base(Constants.FieldCompilers.Normal)
        {
        }

        public override bool IsExclusive => true;

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            return string.Equals(field.TemplateField.Type, "checkbox", StringComparison.OrdinalIgnoreCase);
        }

        public override string Compile(IFieldCompileContext context, Field field)
        {
            var value = field.Value.Trim();

            if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
            {
                return "1";
            }

            if (value == "1")
            {
                return "1";
            }

            return string.Empty;
        }
    }
}
