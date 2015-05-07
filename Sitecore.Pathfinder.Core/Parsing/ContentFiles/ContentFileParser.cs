namespace Sitecore.Pathfinder.Parsing.ContentFiles
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.ContentFiles;

  [Export(typeof(IParser))]
  public class ContentFileParser : ParserBase
  {
    public ContentFileParser() : base(ContentFiles)
    {
    }

    public override bool CanParse(IParseContext context, ISourceFile sourceFile)
    {
      return false;
    }

    public override void Parse(IParseContext context, ISourceFile sourceFile)
    {
      var contentFileModel = new ContentFile(sourceFile);
      context.Project.Elements.Add(contentFileModel);
    }
  }
}
