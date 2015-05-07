namespace Sitecore.Pathfinder.Parsing.MediaFiles
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.MediaFiles;

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

    public MediaFileParser() : base(Media)
    {
    }

    public override bool CanParse(IParseContext context, ISourceFile sourceFile)
    {
      var fileExtension = Path.GetExtension(sourceFile.SourceFileName) ?? string.Empty;
      return FileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context, ISourceFile sourceFile)
    {
      var mediaFile = new MediaFile(sourceFile);
      context.Project.Elements.Add(mediaFile);

      // todo: set template
      mediaFile.Name = Path.GetFileNameWithoutExtension(sourceFile.SourceFileName);
      mediaFile.DatabaseName = context.GetDatabaseName(sourceFile);
      mediaFile.ItemIdOrPath = context.GetItemPath(sourceFile);
    }
  }
}
