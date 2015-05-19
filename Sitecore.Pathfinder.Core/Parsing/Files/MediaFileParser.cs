namespace Sitecore.Pathfinder.Parsing.Files
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using Sitecore.Pathfinder.Projects.Files;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.TextDocuments;

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
      var fileExtension = Path.GetExtension(context.Document.SourceFile.FileName);
      return FileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var mediaItem = new Item(context.Project, context.ItemPath, context.Document)
      {
        ItemName = context.ItemName,
        ItemIdOrPath = context.ItemPath,
        DatabaseName = context.DatabaseName,
        IsEmittable = false,
        OverwriteWhenMerging = true,
        MergingMatch = MergingMatch.MatchUsingSourceFile
      };

      mediaItem = context.Project.AddOrMerge(mediaItem);

      var mediaFile = new MediaFile(context.Project, context.Document, mediaItem);
      context.Project.AddOrMerge(mediaFile);
    }
  }
}