namespace Sitecore.Pathfinder.Parsing.Files
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Projects.Files;
  using Sitecore.Pathfinder.TreeNodes;

  [Export(typeof(IParser))]
  public class ContentFileParser : ParserBase
  {
    public ContentFileParser() : base(ContentFiles)
    {
    }

    public override bool CanParse(IParseContext context)
    {
      return false;
    }

    public override void Parse(IParseContext context)
    {
      var contentFileModel = new ContentFile(context.Project, new TextSpan(context.Document));
      context.Project.Items.Add(contentFileModel);
    }
  }
}
