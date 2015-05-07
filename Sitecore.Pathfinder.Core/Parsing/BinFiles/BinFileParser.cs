namespace Sitecore.Pathfinder.Parsing.BinFiles
{
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Models.BinFiles;

  [Export(typeof(IParser))]
  public class BinFileParser : ParserBase
  {
    public BinFileParser() : base(BinFiles)
    {
    }

    public override void Parse(IParseContext context)
    {
      var contentDirectory = Path.Combine(context.Project.ProjectDirectory, "content");
      var binDirectory = Path.Combine(contentDirectory, "bin");
      if (!context.FileSystem.DirectoryExists(binDirectory))
      {
        return;
      }

      foreach (var fileName in context.FileSystem.GetFiles(binDirectory, "*.dll"))
      {
        var destinationFileName = Path.Combine("bin", Path.GetFileName(fileName) ?? string.Empty);

        var binFileModel = new BinFileModel(fileName, destinationFileName);
        context.Project.Models.Add(binFileModel);
      }
    }
  }
}
