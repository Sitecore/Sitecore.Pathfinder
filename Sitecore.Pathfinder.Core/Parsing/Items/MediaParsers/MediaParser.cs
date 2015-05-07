namespace Sitecore.Pathfinder.Parsing.Items.MediaParsers
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using Sitecore.Pathfinder.Models.MediaItems;

  [Export(typeof(IItemParser))]
  public class MediaParser : ItemParserBase
  {
    private static readonly string[] FileExtensions = {
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

    public MediaParser() : base(Media)
    {
    }

    public override bool CanParse(IItemParseContext context)
    {
      var fileExtension = Path.GetExtension(context.FileName) ?? string.Empty;
      return FileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
    }

    public override void Parse(IItemParseContext context)
    {
      var itemModel = new MediaFileModel(context.FileName);
      context.ParseContext.Project.Models.Add(itemModel);

      // todo: set template
      itemModel.MediaFile = context.FileName;
      itemModel.Name = Path.GetFileNameWithoutExtension(context.FileName);
      itemModel.DatabaseName = context.DatabaseName;
      itemModel.ItemIdOrPath = context.ItemPath;
    }
  }
}
