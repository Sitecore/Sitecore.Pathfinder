namespace Sitecore.Pathfinder.Parsing.Files
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.Files;
  using Sitecore.Pathfinder.Projects.Items;

  [Export(typeof(IParser))]
  public class MediaFileParser : ParserBase
  {
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
      mediaItem.ItemName.Source = new FileNameTextNode(context.ItemName, context.Snapshot);
      mediaItem.IsEmittable = false;
      mediaItem.OverwriteWhenMerging = true;
      mediaItem.MergingMatch = MergingMatch.MatchUsingSourceFile;

      mediaItem = context.Project.AddOrMerge(mediaItem);

      var mediaFile = context.Factory.MediaFile(context.Project, context.Snapshot, mediaItem);
      context.Project.AddOrMerge(mediaFile);
    }
  }
}