// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.Globalization;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    [Export(typeof(IFieldCompileContext))]
    public class FieldCompileContext : IFieldCompileContext
    {
        [FactoryConstructor]
        [ImportingConstructor]
        public FieldCompileContext([NotNull] IConfiguration configuration, [NotNull, ImportMany, ItemNotNull] IEnumerable<IFieldCompiler> fieldCompilers)
        {
            FieldCompilers = fieldCompilers;

            Culture = configuration.GetCulture();
        }

        public CultureInfo Culture { get; }

        public IEnumerable<IFieldCompiler> FieldCompilers { get; }
    }
}
