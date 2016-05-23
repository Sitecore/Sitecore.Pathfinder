// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Languages.Media
{
    public class MediaFileParser : ParserBase
    {
        public MediaFileParser() : base(Constants.Parsers.Media)
        {
        }

        public override bool CanParse(IParseContext context)
        {
            var extension = Path.GetExtension(context.Snapshot.SourceFile.AbsoluteFileName).TrimStart('.').ToLowerInvariant();
            var templateIdOrPath = context.Configuration.GetString(Constants.Configuration.BuildProjectMediaTemplate + ":" + extension);

            return !string.IsNullOrEmpty(templateIdOrPath);
        }

        public override void Parse(IParseContext context)
        {
            var mediaFile = context.Factory.MediaFile(context.Project, context.Snapshot, context.DatabaseName, context.ItemName, context.ItemPath, context.FilePath);
            mediaFile.UploadMedia = context.UploadMedia;
            context.Project.AddOrMerge(mediaFile);

            context.Project.Ducats += 100;
        }
    }
}
