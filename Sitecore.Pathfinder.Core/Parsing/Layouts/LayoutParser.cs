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

    public LayoutParser() : base(Renderings)
    {
    }

    public override bool CanParse(IParseContext context)
    {
      return context.SourceFile.SourceFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var item = new Item(context.SourceFile);
      context.Project.Items.Add(item);

      item.ItemName = context.ItemName;
      item.DatabaseName = context.DatabaseName;
      item.ItemIdOrPath = context.ItemPath;
      item.IsEmittable = false;

      var layout = new Layout(context.SourceFile, item);
      context.Project.Items.Add(layout);
    }
  }
}
