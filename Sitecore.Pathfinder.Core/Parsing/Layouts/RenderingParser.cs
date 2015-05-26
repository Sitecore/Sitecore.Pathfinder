namespace Sitecore.Pathfinder.Parsing.Layouts
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.Projects.Layouts;
  using Sitecore.Pathfinder.Projects.References;

  public abstract class RenderingParser : ParserBase
  {
    protected RenderingParser([NotNull] string fileExtension, [NotNull] string templateIdOrPath) : base(Constants.Parsers.Renderings)
    {
      this.FileExtension = fileExtension;
      this.TemplateIdOrPath = templateIdOrPath;
    }

    [NotNull]
    public string TemplateIdOrPath { get; }

    [NotNull]
    protected string FileExtension { get; }

    public override bool CanParse(IParseContext context)
    {
      return context.DocumentSnapshot.SourceFile.FileName.EndsWith(this.FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var contents = context.DocumentSnapshot.SourceFile.ReadAsText();
      var placeHolders = this.GetPlaceholders(contents);
      var path = "/" + PathHelper.NormalizeItemPath(Path.Combine(context.Configuration.Get(Constants.Configuration.ProjectDirectory), PathHelper.UnmapPath(context.Project.Options.ProjectDirectory, context.DocumentSnapshot.SourceFile.FileName)));

      var item = new Item(context.Project, context.ItemPath, context.DocumentSnapshot)
      {
        ItemName = context.ItemName, 
        ItemIdOrPath = context.ItemPath, 
        DatabaseName = context.DatabaseName, 
        TemplateIdOrPath = this.TemplateIdOrPath, 
        OverwriteWhenMerging = true
      };

      item.Fields.Add(new Field(item.TextNode, "Path", path));
      item.Fields.Add(new Field(item.TextNode, "Place Holders", string.Join(",", placeHolders)));

      item.References.Add(new Reference(item, item.TextNode, path));

      item = context.Project.AddOrMerge(item);

      var rendering = new Rendering(context.Project, context.DocumentSnapshot, item);
      context.Project.AddOrMerge(rendering);
    }

    [NotNull]
    protected abstract IEnumerable<string> GetPlaceholders([NotNull] string contents);
  }
}
