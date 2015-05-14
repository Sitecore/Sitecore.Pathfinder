namespace Sitecore.Pathfinder.Parsing.Other
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Other;
  using Sitecore.Pathfinder.Projects.Templates;

  [Export(typeof(IParser))]
  public class PageTypeParser : ParserBase
  {
    private const string FileExtension = ".pagetype.xml";

    public PageTypeParser() : base(Templates)
    {
    }

    public override bool CanParse(IParseContext context)
    {
      return context.TextDocument.SourceFile.SourceFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var root = context.TextDocument.Root;

      var baseTemplates = new List<string>();
      foreach (var treeNode in root.ChildNodes)
      {
        if (treeNode.Name != "Component")
        {
          throw new BuildException(Texts.Text2032, treeNode);
        }

        var componentPath = treeNode.GetAttributeValue("Component");
        if (string.IsNullOrEmpty(componentPath))
        {
          throw new BuildException(Texts.Text2033, treeNode);
        }

        baseTemplates.Add(componentPath);
      }

      var template = new Template(context.Project, context.ItemName, root)
      {
        ItemName = context.ItemName,
        DatabaseName = context.DatabaseName,
        ItemIdOrPath = context.ItemPath,
        BaseTemplates = string.Join("|", baseTemplates)
      };
      context.Project.Items.Add(template);

      var pageType = new PageType(context.Project, root);
      context.Project.Items.Add(pageType);
    }
  }
}
