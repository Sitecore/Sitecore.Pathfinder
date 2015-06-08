namespace Sitecore.Pathfinder.Parsing.Layouts
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

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

      var item = context.Factory.Item(context.Project, context.ItemPath, context.Snapshot);
      item.ItemName = context.ItemName;
      item.ItemIdOrPath = context.ItemPath;
      item.TemplateIdOrPath = this.TemplateIdOrPath;
      item.DatabaseName = context.DatabaseName;
      item.OverwriteWhenMerging = true;

      var valueTextNode = context.Factory.TextNode(item.Snapshot, string.Empty, path, null);
      item.Fields.Add(context.Factory.Field(item, "Path", string.Empty, 0, valueTextNode, valueTextNode));

      // todo: make this configurable
      if (string.Compare(context.DatabaseName, "core", StringComparison.OrdinalIgnoreCase) == 0)
      {
        var valueTextNode2 = context.Factory.TextNode(item.Snapshot, string.Empty, string.Join(",", placeHolders), null);
        item.Fields.Add(context.Factory.Field(item, "Place Holders", string.Empty, 0, valueTextNode2, valueTextNode2));
      }

      item.References.Add(context.Factory.FileReference(item, item.TextNode, path));

      item = context.Project.AddOrMerge(item);

      var rendering = context.Factory.Rendering(context.Project, context.Snapshot, item);
      context.Project.AddOrMerge(rendering);
    }

    [NotNull]
    protected abstract IEnumerable<string> GetPlaceholders([NotNull] string contents);
  }
}
