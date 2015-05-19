namespace Sitecore.Pathfinder.Parsing.Layouts
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.Projects.Layouts;

  [Export(typeof(IParser))]
  public class LayoutParser : ParserBase
  {
    private const string FileExtension = ".layout.xml";

    public LayoutParser() : base(Constants.Parsers.Renderings)
    {
    }

    public override bool CanParse(IParseContext context)
    {
      return context.Document.SourceFile.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var item = new Item(context.Project, context.ItemPath, context.Document)
      {
        ItemName = context.ItemName,
        DatabaseName = context.DatabaseName,
        ItemIdOrPath = context.ItemPath
      };

      context.Project.AddOrMerge(item);

      var layout = new Layout(context.Project, context.Document);
      context.Project.AddOrMerge(layout);
    }
  }
}