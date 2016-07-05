// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Languages.Media;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checkers
{
    public class FilesChecker : Checker
    {
        [Export("Check")]
        public IEnumerable<Diagnostic> AvoidLargeMediaFiles(ICheckerContext context)
        {
            return from mediaFile in context.Project.ProjectItems.OfType<MediaFile>()
                let fileInfo = new FileInfo(mediaFile.Snapshot.SourceFile.AbsoluteFileName)
                where fileInfo.Length > 5 * 1025 * 1025
                select Warning(Msg.C1027, "Media file size exceeds 5MB. Consider reducing the size of the file", mediaFile.Snapshot.SourceFile.AbsoluteFileName, TextSpan.Empty);
        }
    }
}
