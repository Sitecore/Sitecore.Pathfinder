// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.Files
{
    [Export(typeof(IParser))]
    public class MediaFileParser : ParserBase
    {
        // todo: make this configurable
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
            var fileExtension = Path.GetExtension(context.Snapshot.SourceFile.FileName);
            return FileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        public override void Parse(IParseContext context)
        {
            var mediaItem = context.Factory.Item(context.Project, context.ItemPath, new SnapshotTextNode(context.Snapshot), context.DatabaseName, context.ItemName, context.ItemPath, string.Empty);
            mediaItem.ItemNameProperty.AddSourceTextNode(new FileNameTextNode(context.ItemName, context.Snapshot));
            mediaItem.IsEmittable = false;
            mediaItem.OverwriteWhenMerging = true;
            mediaItem.MergingMatch = MergingMatch.MatchUsingSourceFile;

            mediaItem = context.Project.AddOrMerge(context, mediaItem);

            var mediaFile = context.Factory.MediaFile(context.Project, context.Snapshot, context.FilePath, mediaItem);
            context.Project.AddOrMerge(context, mediaFile);

            context.Project.Ducats += 100;
        }
    }
}
