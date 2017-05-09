// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Emitting.Emitters.WebsiteEmitter
{
    [Export(typeof(IProjectEmitter)), Shared]
    public class WebsiteProjectEmitter : ProjectEmitterBase
    {
        [ImportingConstructor]
        public WebsiteProjectEmitter([NotNull] IConfiguration configuration, [NotNull] ITraceService traceService, [ItemNotNull, NotNull, ImportMany] IEnumerable<IEmitter> emitters, [NotNull] IFileSystemService fileSystem) : base(configuration, traceService, emitters)
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        public IFileSystemService FileSystem { get; }

        public override bool CanEmit(string format)
        {
            return string.Equals(format, "website", StringComparison.OrdinalIgnoreCase);
        }

        public void Copy([NotNull] string sourceFileAbsoluteFileName, [NotNull] string destinationFileName)
        {
            var forceUpdate = Configuration.GetBool(Constants.Configuration.BuildProject.ForceUpdate, true);
            FileSystem.CreateDirectoryFromFileName(destinationFileName);
            FileSystem.Copy(sourceFileAbsoluteFileName, destinationFileName, forceUpdate);
        }
    }
}
