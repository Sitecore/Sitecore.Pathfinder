namespace Sitecore.Pathfinder.Parsing.Layouts
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.Projects.Layouts;
  using Sitecore.Pathfinder.TextDocuments;

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
      return context.Document.SourceFile.SourceFileName.EndsWith(this.FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var textDocument = context.Document as ITextDocument;
      if (textDocument == null)
      {
        throw new BuildException(Texts.Text3031, context.Document);
      }

      var path = "/" + PathHelper.NormalizeItemPath(Path.Combine(context.Configuration.Get(Constants.ProjectDirectory), PathHelper.UnmapPath(context.Project.ProjectDirectory, context.Document.SourceFile.SourceFileName)));
      var placeHolders = this.GetPlaceholders(textDocument.Contents);

      var item = new Item(context.Project, context.ItemPath, textDocument.Root)
      {
        ItemName = context.ItemName, 
        ItemIdOrPath = context.ItemPath, 
        DatabaseName = context.DatabaseName, 
        TemplateIdOrPath = this.TemplateIdOrPath, 
        OverwriteWhenMerging = true
      };

      item.Fields.Add(new Field(textDocument.Root, "Path", path));
      item.Fields.Add(new Field(textDocument.Root, "Place Holders", string.Join(",", placeHolders)));

      item = context.Project.AddOrMerge(item);

      var rendering = new Rendering(context.Project, context.Document, item);
      context.Project.AddOrMerge(rendering);
    }

    [NotNull]
    protected abstract IEnumerable<string> GetPlaceholders([NotNull] string contents);
  }
}
