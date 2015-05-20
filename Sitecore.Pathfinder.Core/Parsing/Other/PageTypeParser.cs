namespace Sitecore.Pathfinder.Parsing.Other
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
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
      var textDocument = context.Document as ITextDocument;
      if (textDocument == null)
      {
        throw new BuildException("public const string Text document expected", context.Document);
      }

      var root = textDocument.Root;

      var baseTemplates = new List<string>();
      foreach (var treeNode in root.ChildNodes)
      {
        if (treeNode.Name != "Component")
        {
          throw new BuildException("'Component' element expected", treeNode);
        }

        var componentPath = treeNode.GetAttributeValue("Component");
        if (string.IsNullOrEmpty(componentPath))
        {
          throw new BuildException("'Component' attribute expected", treeNode);
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
