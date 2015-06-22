namespace Sitecore.Pathfinder.Parsing.Components
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IParser))]
  public class ComponentParser : ParserBase
  {
    private const string FileExtension = ".component.xml";

    public ComponentParser() : base(ContentFiles)
    {
    }

    public override bool CanParse(IParseContext context, ISourceFile sourceFile)
    {
      return sourceFile.SourceFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context, ISourceFile sourceFile)
    {

    }
  }
}