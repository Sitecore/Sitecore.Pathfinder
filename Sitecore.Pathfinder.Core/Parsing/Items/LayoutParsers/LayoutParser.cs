namespace Sitecore.Pathfinder.Parsing.Items.LayoutParsers
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Models.Layouts;

  [Export(typeof(IItemParser))]
  public class LayoutParser : ItemParserBase
  {
    private const string FileExtension = ".layout.xml";

    public LayoutParser() : base(Layout)
    {
    }

    public override bool CanParse(IItemParseContext context)
    {
      return context.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IItemParseContext context)
    {
      var itemModel = new LayoutModel(context.FileName);
      context.ParseContext.Project.Models.Add(itemModel);

      itemModel.Name = context.ItemName;
      itemModel.DatabaseName = context.DatabaseName;
      itemModel.ItemIdOrPath = context.ItemPath;
    }
  }
}
