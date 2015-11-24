// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    public class FileNameFieldCompiler : FieldCompilerBase
    {
        public FileNameFieldCompiler() : base(Constants.FieldCompilers.Normal)
        {
        }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            if (!field.Value.StartsWith("~", StringComparison.Ordinal))
            {
                return false;
            }

            if (field.ValueProperty.Flags.HasFlag(SourcePropertyFlags.IsFileName))
            {
                return true;
            }

            // guess: may actually not be a file name
            if (field.FieldName == "Path")
            {
                return true;
            }

            return false;
        }

        public override string Compile(IFieldCompileContext context, Field field)
        {
            return field.Value.TrimStart('~');
        }
    }
}
