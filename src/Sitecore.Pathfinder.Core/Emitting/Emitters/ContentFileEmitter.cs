// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Content;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitting.Emitters
{
    public class ContentFileEmitter : EmitterBase
    {
        public ContentFileEmitter() : base(Constants.Emitters.BinFiles)
        {
        }

        public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
        {
            return projectItem is ContentFile;
        }

        public override void Emit(IEmitContext context, IProjectItem projectItem)
        {
            var contentFile = (ContentFile)projectItem;

            var filePath = PathHelper.NormalizeFilePath(contentFile.FilePath);
            if (filePath.StartsWith("~\\"))
            {
                filePath = filePath.Mid(2);
            }

            var destinationFileName = PathHelper.Combine(context.Configuration.GetWebsiteDirectory(), filePath);

            context.Trace.TraceInformation(Msg.I1011, "Publishing content file", "~\\" + filePath);

            context.FileSystem.CreateDirectoryFromFileName(destinationFileName);
            context.FileSystem.Copy(projectItem.Snapshot.SourceFile.AbsoluteFileName, destinationFileName, context.ForceUpdate);
        }
    }
}
