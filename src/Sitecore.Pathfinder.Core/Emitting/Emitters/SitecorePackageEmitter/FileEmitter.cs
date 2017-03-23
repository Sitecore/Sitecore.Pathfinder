// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Languages.Content;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitting.Emitters.SitecorePackageEmitter
{
    [Export(typeof(IEmitter)), Shared]
    public class FileEmitter : EmitterBase
    {
        public FileEmitter() : base(Constants.Emitters.ContentFiles)
        {
        }

        public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
        {
            return context.ProjectEmitter is PackageProjectEmitter && projectItem is ContentFile;
        }

        public override void Emit(IEmitContext context, IProjectItem projectItem)
        {
            var contentFile = (ContentFile)projectItem;
            var projectEmitter = (PackageProjectEmitter)context.ProjectEmitter;

            projectEmitter.AddFile(context, projectItem.Snapshot.SourceFile.AbsoluteFileName, contentFile.FilePath);
        }
    }
}
