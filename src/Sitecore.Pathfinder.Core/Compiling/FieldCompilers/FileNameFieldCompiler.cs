using System;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    [Export(typeof(IFieldCompiler)), Shared]
    public class FileNameFieldCompiler : FieldCompilerBase
    {
        [ItemNotNull, NotNull]
        private readonly string[] _pathFields;

        [ImportingConstructor]
        public FileNameFieldCompiler([NotNull] IConfiguration configuration) : base(Constants.FieldCompilers.Normal)
        {
            _pathFields = configuration.GetArray(Constants.Configuration.CheckProject.PathFields);
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

            if (_pathFields.Contains(field.FieldId.Format()))
            {
                return true;
            }

            return false;
        }

        public override string Compile(IFieldCompileContext context, Field field) => PathHelper.NormalizeItemPath(field.Value.TrimStart('~'));
    }
}
