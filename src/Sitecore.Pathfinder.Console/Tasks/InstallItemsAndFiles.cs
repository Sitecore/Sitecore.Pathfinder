// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitting.ItemsAndFilesEmitting;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class InstallItemsAndFiles : BuildTaskBase
    {
        [ImportingConstructor]
        public InstallItemsAndFiles([NotNull] ICompositionService compositionService) : base("install-items-and-files")
        {
            CompositionService = compositionService;
        }

        [NotNull]
        public ICompositionService CompositionService { get; }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.D1027, Texts.Installing_items_and_files___);

            var projectEmitter = CompositionService.Resolve<ItemsAndFilesProjectEmitter>();
            var project = context.LoadProject();

            projectEmitter.Emit(project);
        }
    }
}
