namespace Sitecore.Pathfinder.Parsing.Layouts
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.Projects.Layouts;

  public abstract class RenderingParser : ParserBase
  {
    protected RenderingParser([NotNull] string fileExtension, [NotNull] string templateIdOrPath) : base(Renderings)
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
      var item = new Item(context.Project, context.Document.Root);
      context.Project.Items.Add(item);

      item.ProjectId = "{" + context.ItemPath + "}";
      item.ItemName = context.ItemName;
      item.DatabaseName = context.DatabaseName;
      item.TemplateIdOrPath = this.TemplateIdOrPath;
      item.ItemIdOrPath = context.ItemPath;

      var path = "/" + PathHelper.NormalizeItemPath(Path.Combine(context.Configuration.Get(Constants.ProjectDirectory), context.GetRelativeFileName(context.Document.SourceFile)));
      var placeHolders = this.GetPlaceholders(context, context.Document.SourceFile);

      item.Fields.Add(new Field(context.Document.Root, "Path", path));
      item.Fields.Add(new Field(context.Document.Root, "Place Holders", string.Join(",", placeHolders)));

      var rendering = new Rendering(context.Project, context.Document.Root, item);
      context.Project.Items.Add(rendering);
    }

    [NotNull]
    protected abstract IEnumerable<string> GetPlaceholders([NotNull] IParseContext context, [NotNull] ISourceFile sourceFile);
  }
}
