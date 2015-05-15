namespace Sitecore.Pathfinder.Parsing.Files
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
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
      var fileExtension = Path.GetExtension(context.TextDocument.SourceFile.SourceFileName);
      return FileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var mediaItem = new Item(context.Project, context.ItemPath, context.TextDocument.Root)
      {
        ItemName = context.ItemName,
        ItemIdOrPath = context.ItemPath,
        DatabaseName = context.DatabaseName,
        IsEmittable = false
      };

      mediaItem = context.Project.AddOrMerge(mediaItem);

      var mediaFile = new MediaFile(context.Project, context.TextDocument.Root, mediaItem);
      context.Project.AddOrMerge(mediaFile);
    }
  }
}