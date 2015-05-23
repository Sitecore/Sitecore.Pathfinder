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
      return context.DocumentSnapshot.SourceFile.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var item = new Item(context.Project, context.ItemPath, context.DocumentSnapshot)
      {
        ItemName = context.ItemName,
        DatabaseName = context.DatabaseName,
        ItemIdOrPath = context.ItemPath
      };

      context.Project.AddOrMerge(item);

      var layout = new Layout(context.Project, context.DocumentSnapshot);
      context.Project.AddOrMerge(layout);
    }
  }
}