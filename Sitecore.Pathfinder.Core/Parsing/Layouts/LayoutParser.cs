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
      return context.Document.SourceFile.SourceFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var item = new Item(context.Project, context.Document.Root);
      context.Project.Items.Add(item);

      item.ProjectId = "{" + context.ItemPath + "}";
      item.ItemName = context.ItemName;
      item.DatabaseName = context.DatabaseName;
      item.ItemIdOrPath = context.ItemPath;
      item.IsEmittable = false;

      var layout = new Layout(context.Project, context.Document.Root, item);
      context.Project.Items.Add(layout);
    }
  }
}
