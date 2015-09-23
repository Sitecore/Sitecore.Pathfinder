using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Items.FieldResolvers
{
    [Export(typeof(IFieldResolver))]
    public class FileNameFieldResolver : FieldResolverBase
    {
        public FileNameFieldResolver() : base(Constants.FieldResolvers.Normal)
        {
        }

        public override bool CanResolve(Field field)
        {
            return (field.ValueProperty.SourcePropertyFlags & SourcePropertyFlags.IsFileName) == SourcePropertyFlags.IsFileName & field.Value.StartsWith("~", StringComparison.Ordinal);
        }

        public override string Resolve(ITraceService trace, Field field)
        {
            return field.Value.TrimStart('~');
        }
    }
}