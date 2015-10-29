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
            return (field.ValueProperty.Flags & SourcePropertyFlags.IsFileName) == SourcePropertyFlags.IsFileName & field.Value.StartsWith("~", StringComparison.Ordinal);
        }

        public override string Compile(IFieldCompileContext context, Field field)
        {
            return field.Value.TrimStart('~');
        }
    }
}
