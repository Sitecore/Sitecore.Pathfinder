namespace Sitecore.Pathfinder.Parsing.Files
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Projects.Files;
  using Sitecore.Pathfinder.TreeNodes;

  [Export(typeof(IParser))]
  public class BinFileParser : ParserBase
  {
    private const string FileExtension = ".dll";

    public BinFileParser() : base(BinFiles)
    {
    }

    public override bool CanParse(IParseContext context)
    {
      return context.Document.SourceFile.SourceFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var binFile = new BinFile(context.Project, new TextSpan(context.Document));
      context.Project.Items.Add(binFile);
    }
  }
}
