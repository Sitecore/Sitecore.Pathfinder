// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using System;
using System.IO;
using System.Linq;

namespace Sitecore.Pathfinder.Parsing.Files
{
    public class MediaFileParser : ParserBase
    {
        // todo: make this configurable
        [NotNull]
        [ItemNotNull]
        private static readonly string[] FileExtensions =
        {
            ".png",
            ".gif",
            ".bmp",
            ".jpg",
            ".jpeg",
            ".docx",
            ".doc",
            ".pdf",
            ".zip",
        };

        public MediaFileParser() : base(Constants.Parsers.Media)
        {
        }

        public override bool CanParse(IParseContext context)
        {
            var fileExtension = Path.GetExtension(context.Snapshot.SourceFile.AbsoluteFileName);
            return FileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        public override void Parse(IParseContext context)
        {
            var mediaFile = context.Factory.MediaFile(context.Project, context.Snapshot, context.DatabaseName, context.ItemName, context.ItemPath, context.FilePath);
            context.Project.AddOrMerge(mediaFile);

            context.Project.Ducats += 100;
        }
    }
}
