// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitting.Emitters.DirectoryEmitter;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitting.Emitters.NugetEmitter
{
    public class NugetProjectEmitter : DirectoryProjectEmitter
    {
        public NugetProjectEmitter([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] ITraceService traceService, [ItemNotNull, NotNull, ImportMany] IEnumerable<IEmitter> emitters, [NotNull] IFileSystemService fileSystem) : base(configuration, compositionService, traceService, emitters, fileSystem)
        {
            OutputDirectory = PathHelper.Combine(Configuration.GetProjectDirectory(), Configuration.GetString(Constants.Configuration.Output.Directory) + "\\temp");
        }

        public override bool CanEmit(string format)
        {
            return string.Equals(format, "nuget", StringComparison.OrdinalIgnoreCase);
        }

        public override void Emit(IEmitContext context, IProject project)
        {
            base.Emit(context, project);

            
        }
    }
}