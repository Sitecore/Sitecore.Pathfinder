// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Extensibility
{
    [Export(typeof(IExtensionContext))]
    public class ExtensionContext : IExtensionContext
    {
        [ImportingConstructor]
        public ExtensionContext([NotNull] IConfiguration configuration, [NotNull] IFileSystem fileSystem, [NotNull] ITraceService trace)
        {
            Configuration = configuration;
            FileSystem = fileSystem;
            Trace = trace;
        }

        public IConfiguration Configuration { get; }

        public IFileSystem FileSystem { get; }

        public ITraceService Trace { get; }
    }
}
