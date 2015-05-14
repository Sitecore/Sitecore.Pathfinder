namespace Sitecore.Pathfinder.Parsing.Files
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Projects.Files;

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
      var contentFileModel = new ContentFile(context.Project, context.TextDocument.Root);
      context.Project.Items.Add(contentFileModel);
    }
  }
}
