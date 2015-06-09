namespace Sitecore.Pathfinder.Parsing.Layouts
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

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
      var snapshotTextNode = new SnapshotTextNode(context.Snapshot);

      var item = context.Factory.Item(context.Project, context.ItemPath, snapshotTextNode, context.DatabaseName, context.ItemName, context.ItemPath, this.TemplateIdOrPath);
      item.ItemName.Source = new FileNameTextNode(context.ItemName, context.Snapshot);
      item.OverwriteWhenMerging = true;

      var field = context.Factory.Field(item, "Path", string.Empty, 0, path);
      item.Fields.Add(field);

      // todo: make this configurable
      if (string.Compare(context.DatabaseName, "core", StringComparison.OrdinalIgnoreCase) == 0)
      {
        field = context.Factory.Field(item, "Place Holders", string.Empty, 0, string.Join(",", placeHolders));
        item.Fields.Add(field);
      }

      item.References.Add(context.Factory.FileReference(item, snapshotTextNode, path));

      item = context.Project.AddOrMerge(item);

      var rendering = context.Factory.Rendering(context.Project, context.Snapshot, item);
      context.Project.AddOrMerge(rendering);
    }

    [NotNull]
    protected abstract IEnumerable<string> GetPlaceholders([NotNull] string contents);
  }
}
