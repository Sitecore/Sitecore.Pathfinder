namespace Sitecore.Pathfinder.Parsing.Other
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Projects.Other;
  using Sitecore.Pathfinder.Projects.Templates;
  using Sitecore.Pathfinder.TextDocuments;

  [Export(typeof(IParser))]
  public class PageTypeParser : ParserBase
  {
    private const string FileExtension = ".pagetype.xml";

    public PageTypeParser() : base(Constants.Parsers.Templates)
    {
    }

    public override bool CanParse(IParseContext context)
    {
      return context.Document.SourceFile.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var textDocument = (ITextDocument)context.Document;
      var root = textDocument.Root;
      if (root == TextNode.Empty)
      {
        context.Trace.TraceError(Texts.Document_is_not_valid, textDocument.SourceFile.FileName, TextPosition.Empty);
        return;
      }

      var baseTemplates = new List<string>();
      foreach (var treeNode in root.ChildNodes)
      {
        if (treeNode.Name != "Component")
        {
          context.Trace.TraceError(Texts._Component__element_expected, treeNode.Document.SourceFile.FileName, treeNode.Position);
          return;
        }

        var componentPath = treeNode.GetAttributeValue("Component");
        if (string.IsNullOrEmpty(componentPath))
        {
          context.Trace.TraceError(Texts._Component__element_expected, treeNode.Document.SourceFile.FileName, treeNode.Position);
          return;
        }

        baseTemplates.Add(componentPath);
      }

      var template = new Template(context.Project, context.ItemPath, root)
      {
        ItemName = context.ItemName, 
        ItemIdOrPath = context.ItemPath, 
        DatabaseName = context.DatabaseName, 
        BaseTemplates = string.Join("|", baseTemplates)
      };

      context.Project.AddOrMerge(template);

      var pageType = new PageType(context.Project, context.Document);
      context.Project.AddOrMerge(pageType);
    }
  }
}
