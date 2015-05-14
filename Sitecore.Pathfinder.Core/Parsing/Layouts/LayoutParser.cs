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
      return context.TextDocument.SourceFile.SourceFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var item = new Item(context.Project, context.ItemName, context.TextDocument.Root)
      {
        ItemName = context.ItemName, 
        DatabaseName = context.DatabaseName, 
        ItemIdOrPath = context.ItemPath, 
        IsEmittable = false
      };

      context.Project.Items.Add(item);

      var layout = new Layout(context.Project, context.TextDocument.Root, item);
      context.Project.Items.Add(layout);
    }
  }
}