// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    [Export(typeof(IFieldCompiler))]
    public class FileNameFieldCompiler : FieldCompilerBase
    {
        public FileNameFieldCompiler() : base(Constants.FieldResolvers.Normal)
        {
        }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            return (field.ValueProperty.SourcePropertyFlags & SourcePropertyFlags.IsFileName) == SourcePropertyFlags.IsFileName & field.Value.StartsWith("~", StringComparison.Ordinal);
        }

        public override string Compile(IFieldCompileContext context, Field field)
        {
            return field.Value.TrimStart('~');
        }
    }
}
