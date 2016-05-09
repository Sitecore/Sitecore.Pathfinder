// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Languages.Media;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers.Files
{
    public class MediaFileChecker : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            foreach (var mediaFile in context.Project.ProjectItems.OfType<MediaFile>())
            {
                CheckTemplate(context, mediaFile);
            }
        }

        private void CheckTemplate([NotNull] ICheckerContext context, [NotNull] MediaFile mediaFile)
        {
            var fileInfo = new FileInfo(mediaFile.Snapshots.First().SourceFile.AbsoluteFileName);

            if (fileInfo.Length > 5 * 1025 * 1025)
            {
                context.Trace.TraceWarning(Msg.C1027, "Media file size exceeds 5MB. Consider reducing the size of the file", mediaFile.Snapshots.First().SourceFile.AbsoluteFileName, TextSpan.Empty);
            }
        }
    }
}
