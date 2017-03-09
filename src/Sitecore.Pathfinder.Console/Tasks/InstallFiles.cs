// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitting.ItemsAndFilesEmitting;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class InstallFiles : BuildTaskBase
    {
        [ImportingConstructor]
        public InstallFiles([NotNull] ICompositionService compositionService) : base("install-files")
        {
            CompositionService = compositionService;
        }

        [NotNull]
        public ICompositionService CompositionService { get; }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.D1028, "Installing files...");

            var projectEmitter = CompositionService.Resolve<FilesProjectEmitter>();
            var project = context.LoadProject();

            projectEmitter.Emit(project);
        }
    }
}
