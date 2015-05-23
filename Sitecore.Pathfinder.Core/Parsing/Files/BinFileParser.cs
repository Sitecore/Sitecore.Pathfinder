namespace Sitecore.Pathfinder.Parsing.Files
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Projects.Files;

  [Export(typeof(IParser))]
  public class BinFileParser : ParserBase
  {
    private const string FileExtension = ".dll";

    public BinFileParser() : base(Constants.Parsers.BinFiles)
    {
    }

    public override bool CanParse(IParseContext context)
    {
      return context.DocumentSnapshot.SourceFile.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var binFile = new BinFile(context.Project, context.DocumentSnapshot);
      context.Project.AddOrMerge(binFile);
    }
  }
}
