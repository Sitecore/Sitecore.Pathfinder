// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Emitting.Emitters.DirectoryEmitter
{
    [Export(typeof(IProjectEmitter)), Shared]
    public class DirectoryProjectEmitter : ProjectEmitterBase
    {
        [ImportingConstructor]
        public DirectoryProjectEmitter([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] ITraceService traceService, [ItemNotNull, NotNull, ImportMany] IEnumerable<IEmitter> emitters, [NotNull] IFileSystemService fileSystem) : base(configuration, compositionService, traceService, emitters)
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        public IFileSystemService FileSystem { get; }

        public override bool CanEmit(string format)
        {
            return string.Equals(format, "directory", StringComparison.OrdinalIgnoreCase);
        }

        public void AddFile([NotNull] IEmitContext context, [NotNull] string sourceFileAbsoluteFileName, [NotNull] string filePath)
        {
            var fileName = PathHelper.NormalizeFilePath(filePath);
            if (fileName.StartsWith("~\\"))
            {
                fileName = fileName.Mid(2);
            }

            context.Trace.TraceInformation(Msg.I1011, "Publishing", "~\\" + fileName);

            var forceUpdate = Configuration.GetBool(Constants.Configuration.BuildProject.ForceUpdate, true);
            var outputDirectory = PathHelper.Combine(context.Configuration.GetProjectDirectory(), context.Configuration.GetString(Constants.Configuration.Output.Directory));
            var destinationFileName = PathHelper.Combine(outputDirectory, fileName);

            FileSystem.CreateDirectoryFromFileName(destinationFileName);
            FileSystem.Copy(sourceFileAbsoluteFileName, destinationFileName, forceUpdate);
        }
    }
}
