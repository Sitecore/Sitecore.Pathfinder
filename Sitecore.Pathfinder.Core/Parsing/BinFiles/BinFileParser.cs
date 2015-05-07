namespace Sitecore.Pathfinder.Parsing.BinFiles
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.BinFiles;

  [Export(typeof(IParser))]
  public class BinFileParser : ParserBase
  {
    private const string FileExtension = ".dll";

    public BinFileParser() : base(BinFiles)
    {
    }

    public override bool CanParse(IParseContext context, ISourceFile sourceFile)
    {
      return sourceFile.SourceFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context, ISourceFile sourceFile)
    {
      var binFile = new BinFile(sourceFile);
      context.Project.Elements.Add(binFile);
    }
  }
}
