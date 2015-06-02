namespace Sitecore.Pathfinder.Parsing.Layouts
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
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
      return context.Snapshot.SourceFile.FileName.EndsWith(this.FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var contents = context.Snapshot.SourceFile.ReadAsText();
      var placeHolders = this.GetPlaceholders(contents);
      var path = context.FilePath;

      var item = new Item(context.Project, context.ItemPath, context.Snapshot)
      {
        ItemName = context.ItemName,
        ItemIdOrPath = context.ItemPath,
        TemplateIdOrPath = this.TemplateIdOrPath,
        DatabaseName = context.DatabaseName,
        OverwriteWhenMerging = true
      };

      var valueTextNode = new TextNode(item.Snapshot, string.Empty, path, null);
      item.Fields.Add(new Field("Path", string.Empty, 0, valueTextNode, valueTextNode));

      // todo: make this configurable
      if (string.Compare(context.DatabaseName, "core", StringComparison.OrdinalIgnoreCase) == 0)
      {
        var valueTextNode2 = new TextNode(item.Snapshot, string.Empty, string.Join(",", placeHolders), null);
        item.Fields.Add(new Field("Place Holders", string.Empty, 0, valueTextNode2, valueTextNode2));
      }

      item.References.Add(new FileReference(item, item.TextNode, path));

      item = context.Project.AddOrMerge(item);

      var rendering = new Rendering(context.Project, context.Snapshot, item);
      context.Project.AddOrMerge(rendering);
    }

    [NotNull]
    protected abstract IEnumerable<string> GetPlaceholders([NotNull] string contents);
  }
}
