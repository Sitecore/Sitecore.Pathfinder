// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;

namespace Sitecore.Pathfinder.Emitters.Files
{
    [Export(typeof(IEmitter))]
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

            var destinationFileName = FileUtil.MapPath(contentFile.FilePath);

            if (context.FileSystem.FileExists(destinationFileName))
            {
                context.RegisterUpdatedFile(contentFile, destinationFileName);
            }
            else
            {
                context.RegisterAddedFile(contentFile, destinationFileName);
            }

            context.FileSystem.CreateDirectory(Path.GetDirectoryName(destinationFileName) ?? string.Empty);
            context.FileSystem.Copy(projectItem.Snapshots.First().SourceFile.FileName, destinationFileName);
        }
    }
}
